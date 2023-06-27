numbers::[Int]
numbers = [1..100]

quickSort::Ord a => [a] -> [a]
quickSort [] = []
quickSort [x] = [x]
quickSort (x:xs) = quickSort l ++ [x] ++ quickSort r
    where
        l = filter (<x) xs
        r = filter (>x) xs