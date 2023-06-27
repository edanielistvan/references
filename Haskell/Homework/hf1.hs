{-# LANGUAGE DeriveFunctor #-}
{-# OPTIONS_GHC -Wincomplete-patterns #-}

import Data.Traversable
import Control.Applicative
import Control.Monad
import Data.String
import Data.Maybe
import Debug.Trace

newtype State s a = State {runState :: s -> (a, s)} deriving Functor

instance Applicative (State s) where
  pure a = State (a,)
  (<*>) = ap

instance Monad (State s) where
  return = pure
  State f >>= g = State (\s -> case f s of (a, s') -> runState (g a) s')

get :: State s s
get = State (\s -> (s, s))

put :: s -> State s ()
put s = State (const ((), s))

modify :: (s -> s) -> State s ()
modify f = do {s <- get; put (f s)}

evalState :: State s a -> s -> a
evalState ma = fst . runState ma

execState :: State s a -> s -> s
execState ma = snd . runState ma

data ProgState = ProgState {
  r1     :: Int,
  r2     :: Int,
  r3     :: Int,
  cmp    :: Ordering,
  memory :: [Int]
  } deriving (Eq, Show)

startState :: ProgState
startState = ProgState 0 0 0 EQ (replicate 10 0)

type Label = String  -- címke a programban, ahová ugrani lehet

data Register
  = R1
  | R2
  | R3
  deriving (Eq, Show)

data Destination
  = DstReg Register     -- regiszterbe írunk
  | DstDeref Register   -- memóriába írunk, az adott regiszterben tárolt index helyére
  deriving (Eq, Show)

data Source
  = SrcReg Register     -- regiszterből olvasunk
  | SrcDeref Register   -- memóriából olvasunk, az adott regiszterben tárolt index helyéről
  | SrcLit Int          -- szám literál
  deriving (Eq, Show)

data Instruction
  = Mov Destination Source   -- írjuk a Destination-be a Source értékét
  | Add Destination Source   -- adjuk a Destination-höz a Source értékét
  | Mul Destination Source   -- szorozzuk a Destination-t a Source értékével
  | Sub Destination Source   -- vonjuk ki a Destination-ből a Source értékét
  | Cmp Source Source        -- hasonlítsunk össze két Source értéket `compare`-el, az eredményt
                             -- írjuk a `cmp` regiszterbe

  | Jeq Label                -- Ugorjunk az adott címkére ha a `cmp` regiszterben `EQ` van
  | Jlt Label                -- Ugorjunk az adott címkére ha a `cmp` regiszterben `LT` van
  | Jgt Label                -- Ugorjunk az adott címkére ha a `cmp` regiszterben `GT` van
  deriving (Eq, Show)

type RawProgram = [Either Label Instruction]
type Program = [(Label, [Instruction])]

p1 :: RawProgram
p1 = [
  Left "start",
  Right $ Mov (DstReg R1) (SrcLit 10),
  Left "l1",
  Right $ Mov (DstReg R2) (SrcLit 20)
  ]

p2 :: RawProgram
p2 = [
  Left "start",
  Right $ Mov (DstReg R1) (SrcLit 10),
  Right $ Mov (DstReg R2) (SrcLit 1),
  Left "loop",
  Right $ Mul (DstReg R2) (SrcReg R1),
  Right $ Sub (DstReg R1) (SrcLit 1),
  Right $ Cmp (SrcReg R1) (SrcLit 0),
  Right $ Jgt "loop"
  ]

p3 :: RawProgram
p3 = [
  Left "start",
  Right $ Mov (DstDeref R1) (SrcReg R1),
  Right $ Add (DstReg R1) (SrcLit 1),
  Right $ Cmp (SrcReg R1) (SrcLit 10),
  Right $ Jlt "start"
  ]

p4 :: RawProgram
p4 = [
  Left "start",
  Right $ Add (DstDeref R2) (SrcLit 1),
  Right $ Add (DstReg R2) (SrcLit 1),
  Right $ Cmp (SrcReg R2) (SrcLit 10),
  Right $ Jlt "start"
  ]

p5 :: RawProgram
p5 = [
  Left "start",
  Right $ Jeq "first",
  Left "first",
  Right $ Add (DstReg R3) (SrcLit 1),
  Left "second",
  Right $ Add (DstReg R3) (SrcLit 1)
  ]

doTakeWhile :: [Either Label Instruction] -> [Instruction]
doTakeWhile [] = []
doTakeWhile (x:xs) = case x of
    Left l -> doTakeWhile xs
    Right r -> r : doTakeWhile xs

toProgram :: RawProgram -> Program
toProgram [] = []
toProgram (x:xs) = case x of
    Left l -> (l, doTakeWhile xs) : toProgram xs
    Right r -> toProgram xs

type M a = State ProgState a

eval :: Program -> [Instruction] -> M ()
eval _ [] = return ()
eval prg (x:xs) = do
    st <- get
    case x of
        Mov d s -> do {put (executeInstruction (\a b -> b) d s st); eval prg xs}
        Add d s -> do {put (executeInstruction (+) d s st); eval prg xs}
        Mul d s -> do {put (executeInstruction (*) d s st); eval prg xs}
        Sub d s -> do {put (executeInstruction (-) d s st); eval prg xs}
        Cmp s1 s2 -> do {put (executeCompare s1 s2 st); eval prg xs}
        Jeq l -> if cmp st == EQ then eval prg (getInstructions prg l) else eval prg xs
        Jlt l -> if cmp st == LT then eval prg (getInstructions prg l) else eval prg xs
        Jgt l -> if cmp st == GT then eval prg (getInstructions prg l) else eval prg xs

executeInstruction :: (Int -> Int -> Int) -> Destination -> Source -> ProgState -> ProgState
executeInstruction f d s prs = setDestination d prs (f (getDestination d prs) (getSource s prs))

executeCompare :: Source -> Source -> ProgState -> ProgState
executeCompare s1 s2 prs
  | i1 == i2 = prs {cmp = EQ}
  | i1 < i2 = prs {cmp = LT} 
  | otherwise = prs {cmp = GT}
    where
        i1 = getSource s1 prs
        i2 = getSource s2 prs

getDestination :: Destination -> ProgState -> Int
getDestination (DstReg r) prs = case r of
    R1 -> r1 prs
    R2 -> r2 prs
    R3 -> r3 prs
getDestination (DstDeref r) prs = memory prs !! ind
    where
        ind = case r of
            R1 -> r1 prs
            R2 -> r2 prs
            R3 -> r3 prs

setDestination :: Destination -> ProgState -> Int -> ProgState
setDestination (DstReg r) prs n = case r of
    R1 -> prs {r1 = n}
    R2 -> prs {r2 = n}
    R3 -> prs {r3 = n}
setDestination (DstDeref r) prs n = prs {memory = mem}
    where
        mem = take ind (memory prs) ++ [n] ++ drop (ind + 1) (memory prs)
        ind = case r of
            R1 -> r1 prs
            R2 -> r2 prs
            R3 -> r3 prs

getSource :: Source -> ProgState -> Int
getSource (SrcLit n) _ = n
getSource (SrcReg r) prs = case r of
    R1 -> r1 prs
    R2 -> r2 prs
    R3 -> r3 prs
getSource (SrcDeref r) prs = memory prs !! ind
    where
        ind = case r of
            R1 -> r1 prs
            R2 -> r2 prs
            R3 -> r3 prs

getInstructions :: Program -> Label -> [Instruction]
getInstructions [] _ = []
getInstructions (x:xs) l = if fst x == l then snd x else getInstructions xs l

runProgram :: RawProgram -> ProgState
runProgram rprog = case toProgram rprog of
  []                  -> startState
  prog@((_, start):_) -> execState (eval prog start) startState