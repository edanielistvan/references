{-# options_ghc -Wincomplete-patterns #-}
{-# language DeriveFunctor, LambdaCase #-}

import Data.Char
import Data.Maybe
import Control.Applicative
import Control.Monad

newtype Parser a = Parser {runParser :: String -> Maybe (a, String)}
  deriving Functor

instance Applicative Parser where
  pure a = Parser $ \s -> Just (a, s)
  (<*>) = ap

instance Monad Parser where
  return = pure
  Parser f >>= g = Parser $ \s -> case f s of
    Nothing     -> Nothing
    Just (a, s) -> runParser (g a) s

eof :: Parser ()
eof = Parser $ \case
  "" -> Just ((), "")
  _  -> Nothing

-- egy karaktert olvassunk az input elejéről, amire
-- igaz egy feltétel
satisfy :: (Char -> Bool) -> Parser Char
satisfy f = Parser $ \case
  c:s | f c -> Just (c, s)
  _         -> Nothing

-- olvassunk egy konkrét karaktert
char :: Char -> Parser ()
char c = void (satisfy (==c))
  -- satisfy (==c)   hiba: Parser Char helyett Parser () kéne

instance Alternative Parser where
  empty = Parser $ const Nothing
  (<|>) (Parser f) (Parser g) = Parser $ \s -> case f s of
    Nothing -> g s
    x       -> x

-- konkrét String olvasása:
string :: String -> Parser ()
string = mapM_ char -- minden karakterre alkalmazzuk a char-t

-- standard függvények (Control.Applicative-ból)
-- many :: Parser a -> Parser [a]
--    (0-szor vagy többször futtatunk egy parser-t)
-- some :: Parser a -> Parser [a]
--    (1-szor vagy többször futtatunk egy parser-t)

many_ :: Parser a -> Parser ()
many_ pa = void (many pa)

some_ :: Parser a -> Parser ()
some_ pa = void (some pa)

-- olvassunk 0 vagy több pa-t, psep-el elválasztva
sepBy :: Parser a -> Parser sep -> Parser [a]
sepBy pa psep = sepBy1 pa psep <|> pure []

-- olvassunk 1 vagy több pa-t, psep-el elválasztva
sepBy1 :: Parser a -> Parser sep -> Parser [a]
sepBy1 pa psep = (:) <$> pa <*> many (psep *> pa)

pDigit :: Parser Int
pDigit = digitToInt <$> satisfy isDigit

-- pozitív Int olvasása
pPos :: Parser Int
pPos = do
  ds <- some pDigit
  pure $ sum $ zipWith (*) (reverse ds) (iterate (*10) 1)

rightAssoc :: (a -> a -> a) -> Parser a            -> Parser sep -> Parser a
rightAssoc f pa psep = foldr1 f <$> sepBy1 pa psep

leftAssoc :: (a -> a -> a) -> Parser a -> Parser sep -> Parser a
leftAssoc f pa psep = foldl1 f <$> sepBy1 pa psep

nonAssoc :: (a -> a -> a) -> Parser a -> Parser sep -> Parser a
nonAssoc f pa psep = do
  exps <- sepBy1 pa psep
  case exps of
    [e]      -> pure e
    [e1,e2]  -> pure (f e1 e2)
    _        -> empty

-- nem láncolható prefix operátor
prefix :: (a -> a) -> Parser a -> Parser op -> Parser a
prefix f pa pop = (pop *> (f <$> pa)) <|> pa

anyChar :: Parser Char
anyChar = satisfy (const True)

optional_ :: Parser a -> Parser ()
optional_ p = void (optional p)

-- A következő regexek támogatottak:
data RegEx
  -- Atomok:
  -- - (p) : (nincs külön konstruktora,
  --         hiszen a zárójelek nem jelennek meg az absztrakt szintaxisfában)
  -- - a : Karakter literál, amely betű, szóköz vagy szám lehet
  = REChar Char
  -- - [c1-c2] : Két karakter által meghatározott (mindkét oldalról zárt) intervallum
  --             Példák: [a-z], [0-9], ...
  | RERange Char Char
  -- - . : Tetszőleges karakter
  | REAny
  -- - $ : Üres bemenet ("End of file")
  | REEof

  -- Posztfix operátorok:
  -- - p* : Nulla vagy több ismétlés
  | REMany RegEx
  -- - p+ : Egy vagy több ismétlés
  | RESome RegEx
  -- - p? : Nulla vagy egy előfordulás
  | REOptional RegEx
  -- - p{n} : N-szeres ismétlés
  | RERepeat RegEx Int

  -- Infix operátorok:
  -- - Regex-ek egymás után futtatása.
  --   Jobbra asszociáló infix művelet, a szintaxisban "láthatatlan", egyszerűen
  --   egymás után írunk több regexet.
  | RESequence RegEx RegEx
  -- - p1|p2 : Először p1 futtatása, ha az nem illeszkedik, akkor p2.
  -- - Jobbra asszociál.
  | REChoice RegEx RegEx
  deriving (Eq, Show)

-- 1. Feladat
topLevel :: Parser a -> Parser a
topLevel pa = ws *> pa <* eof

ws :: Parser ()
ws = many_ (char ' ' <|> char '\n')

readChar :: Parser Char
readChar = satisfy (\c -> c == ' ' || isDigit c || isLetter c)

postfix :: (a -> a) -> Parser a -> Parser op -> Parser a
postfix f pa pop = (f <$> pa) <* pop

-- Kotesi erosseg:
-- 1. atomok
-- 2. postfix operatorok
-- 3. seq
-- 4. p1|p2

pRegEx :: Parser RegEx
pRegEx = topLevel pChoice

pChoice :: Parser RegEx
pChoice = rightAssoc REChoice pSeq (char '|')

pSeq :: Parser RegEx
pSeq = rightAssoc RESequence pPostfix (string "")

pPostfix :: Parser RegEx
pPostfix = ((RERepeat <$> pAtom) <*> (char '{' *> pPos <* char '}')) 
       <|> postfix REOptional pAtom (char '?')
       <|> postfix RESome pAtom (char '+') 
       <|> postfix REMany pAtom (char '*')
       <|> pAtom

pAtom :: Parser RegEx
pAtom = char '(' *> pChoice <* char ')' 
    <|> RERange <$> (char '[' *> readChar <* char '-') <*> (readChar <* char ']')
    <|> REAny <$ char '.' 
    <|> REEof <$ char '$'
    <|> REChar <$> readChar

-- 2. Feladat
repeat_ :: Parser a -> Int -> Parser ()
repeat_ _ 0 = pure ()
repeat_ pa n = pa *> repeat_ pa (n-1) 

makeParser :: RegEx -> Parser ()
makeParser re = case re of
  REChar c -> char c
  RERange c1 c2 -> void (satisfy (\c -> c1 <= c && c <= c2))
  REAny -> void (satisfy (const True))
  REEof -> eof
  REMany re -> many_ (makeParser re)
  RESome re -> some_ (makeParser re)
  REOptional re -> optional_ (makeParser re)
  RERepeat re n -> repeat_ (makeParser re) n
  RESequence r1 r2 -> makeParser r1 *> makeParser r2
  REChoice r1 r2 -> makeParser r1 <|> makeParser r2

evalParser :: Parser a -> String -> Maybe a
evalParser pre s = case runParser pre s of
  Just ret -> Just (fst ret)
  Nothing -> Nothing

test :: String -> String -> Maybe Bool
test pattern input = do
  regEx <- evalParser pRegEx pattern
  return (isJust (evalParser (makeParser regEx) input))

test' :: String -> String -> Bool
test' regex str = fromJust $ test regex str

licensePlate = "[A-Z]{3}[0-9]{3}$"
hexColor = "0x([0-9]|[A-F]){6}$"
streetName = "([A-Z][a-z]* )+(utca|út) [0-9]+([A-Z])?"

tests :: [Bool]
tests =
  [       test' licensePlate "ABC123"
  ,       test' licensePlate "IRF764"
  ,       test' licensePlate "LGM859"
  ,       test' licensePlate "ASD789"
  , not $ test' licensePlate "ABCD1234"
  , not $ test' licensePlate "ABC123asdf"
  , not $ test' licensePlate "123ABC"
  , not $ test' licensePlate "asdf"

  --

  ,       test' hexColor "0x000000"
  ,       test' hexColor "0x33FE67"
  ,       test' hexColor "0xFA55B8"
  , not $ test' hexColor "1337AB"
  , not $ test' hexColor "0x1234567"
  , not $ test' hexColor "0xAA1Q34"

  --

  ,       test' streetName "Ady Endre út 47C"
  ,       test' streetName "Karinthy Frigyes út 8"
  ,       test' streetName "Budafoki út 3"
  ,       test' streetName "Szilva utca 21A"
  ,       test' streetName "Nagy Lantos Andor utca 9"
  ,       test' streetName "T utca 1"
  , not $ test' streetName "ady Endre út 47C"
  , not $ test' streetName "KarinthyFrigyes út 8"
  , not $ test' streetName "út 3"
  , not $ test' streetName "Liget köz 21A"
  , not $ test' streetName "Nagy  Lantos  Andor utca 9"
  , not $ test' streetName "T utca"
  ] 

add :: IO ()
add = do
  a <- getLine
  b <- getLine
  
  putStrLn (a ++ b)