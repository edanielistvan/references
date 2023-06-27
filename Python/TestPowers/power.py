import numpy as np


class Average:
    def __init__(self):
        self.values = []
        self.length = 0

    def add(self, value):
        self.values.append(value)
        self.length += 1

    def getBest(self):
        return min(self.values)

    def getWorst(self):
        return max(self.values)

    def getMedian(self):
        mid = self.length // 2
        if self.length % 2 == 0:
            return (self.values[mid] + self.values[mid + 1]) / 2
        else:
            return self.values[mid]

    def getMode(self):
        u, c = np.unique(np.array(self.values), return_counts=True)
        maxind = np.argmax(c)
        return u[maxind]

    def getAverage(self):
        return sum(self.values) / self.length


def lopPower(n: int, k: int) -> int:
    if k <= 1:
        return 1

    db = 1
    for i in range(1, k):
        db += 1

    return db


def algPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 2 == 0:
            return 1 + algPower(n * n, k // 2, s)
        else:
            return 1 + algPower(n, k - 1, s + n)


def threeOnlyPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 3 == 0:
            return 1 + threeOnlyPower(n * n * n, k // 3, s)
        elif k % 3 == 1:
            return 1 + threeOnlyPower(n, k - 1, s + n)
        else:
            return 1 + threeOnlyPower(n, k - 2, s + n + n)


def threeAndTwoPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 3 == 0:
            return 1 + threeAndTwoPower(n * n * n, k // 3, s)
        elif k % 2 == 0:
            return 1 + threeAndTwoPower(n * n, k // 2, s)
        elif k % 3 == 1 or k % 2 == 1:
            return 1 + threeAndTwoPower(n, k - 1, s + n)
        else:
            return 1 + threeAndTwoPower(n, k - 2, s + n + n)


def alsoSixPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 6 == 0:
            return 1 + alsoSixPower((n * n) ** 3, k // 6, s)
        elif k % 3 == 0:
            return 1 + alsoSixPower(n * n * n, k // 3, s)
        elif k % 2 == 0:
            return 1 + alsoSixPower(n * n, k // 2, s)
        elif k % 3 == 1 or k % 2 == 1:
            return 1 + alsoSixPower(n, k - 1, s + n)
        else:
            return 1 + alsoSixPower(n, k - 2, s + n + n)


def alsoSixAndFourPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 6 == 0:
            return 1 + alsoSixAndFourPower((n * n) ** 3, k // 6, s)
        if k % 4 == 0:
            return 1 + alsoSixAndFourPower(n * n * n * n, k // 4, s)
        elif k % 3 == 0:
            return 1 + alsoSixAndFourPower(n * n * n, k // 3, s)
        elif k % 2 == 0:
            return 1 + alsoSixAndFourPower(n * n, k // 2, s)
        elif k % 3 == 1 or k % 2 == 1:
            return 1 + alsoSixAndFourPower(n, k - 1, s + n)
        else:
            return 1 + alsoSixAndFourPower(n, k - 2, s + n + n)


def alsoSixAndFourAndNinePower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 9 == 0:
            return 1 + alsoSixAndFourAndNinePower((n * n * n) ** 2, k // 9, s)
        if k % 6 == 0:
            return 1 + alsoSixAndFourAndNinePower((n * n) ** 3, k // 6, s)
        if k % 4 == 0:
            return 1 + alsoSixAndFourAndNinePower(n * n * n * n, k // 4, s)
        elif k % 3 == 0:
            return 1 + alsoSixAndFourAndNinePower(n * n * n, k // 3, s)
        elif k % 2 == 0:
            return 1 + alsoSixAndFourAndNinePower(n * n, k // 2, s)
        elif k % 3 == 1 or k % 2 == 1:
            return 1 + alsoSixAndFourAndNinePower(n, k - 1, s + n)
        else:
            return 1 + alsoSixAndFourAndNinePower(n, k - 2, s + n + n)


def fourOnlyPower(n: int, k: int, s=0) -> int:
    if k == 0:
        return 1
    elif k == 1:
        return 1
    else:
        if k % 4 == 0:
            return 1 + fourOnlyPower((n * n) ** 2, k // 4, s)
        elif k % 4 == 1:
            return 1 + fourOnlyPower(n, k - 1, s + n)
        elif k % 4 == 2:
            return 1 + fourOnlyPower(n * n, k // 2, s)
        elif k % 4 == 3:
            return 1 + fourOnlyPower(n, k - 1, s + n)
        else:
            return 1 + fourOnlyPower(n, k - 2, s + n + n)


def main():
    N = 2
    K = 10000

    names = ["Loop", "Algorithm", "Three only", "Four only",
             "Three and two", "Also six", "Also six and four", "Also six and four and nine"]
    averages = [Average(), Average(), Average(),
                Average(), Average(), Average(), Average(), Average()]

    for i in range(0, K + 1):
        averages[0].add(lopPower(N, i))
        averages[1].add(algPower(N, i))
        averages[2].add(threeOnlyPower(N, i))
        averages[3].add(fourOnlyPower(N, i))
        averages[4].add(threeAndTwoPower(N, i))
        averages[5].add(alsoSixPower(N, i))
        averages[6].add(alsoSixAndFourPower(N, i))
        averages[7].add(alsoSixAndFourAndNinePower(N, i))

    for i in range(0, len(names)):
        print(
            f"{names[i]} - B: {averages[i].getBest()} | A: {averages[i].getAverage()} | Me: {averages[i].getMedian()} | Mo: {averages[i].getMode()} | W: {averages[i].getWorst()}")


main()
