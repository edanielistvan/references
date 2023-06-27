from random import randint
import time
from math import floor

def swap(arr, i, j):
    s = arr[i]
    arr[i] = arr[j]
    arr[j] = s

def sgn(num):
    return -1 if num < 0 else 1 if num > 0 else 0

def partitionTest(array,l,r):
    piv = randint(l,r)
    i = l

    while i <= r:
        if i != piv:
            s = sgn(i-piv)
            if s*array[i] < s*array[piv]:
                swap(array,i,piv+s)
                swap(array,piv,piv+s)
                i=i-1
                piv=piv+s
        i=i+1
    return piv

def partitionTestt(array,l,r):
    piv = randint(l,r)
    swap(array,l,piv)
    lgt = 0
    piv = l

    while(piv+lgt < r):
        nxt = piv+lgt+1
        if array[piv] <= array[nxt]:
            lgt += 1
        else:
            while nxt > piv:
                swap(array,nxt,nxt-1)
                nxt = nxt-1
            piv += 1
    return piv

def partitionTesttt(array,l,r):
    piv = randint(l,r)
    swap(array,l,piv)
    lgt = 0; piv=l

    while(piv+lgt < r):
        nxt = piv+lgt+1
        if array[piv] <= array[nxt]:
            lgt += 1
        else:
            swap(array,nxt,piv+1)
            swap(array,piv,piv+1)
            piv += 1
    return piv

def quickSort(array,l,r):
    if l < r:
        m = partitionTest(array,l,r)
        quickSort(array,l,m-1)
        quickSort(array,m+1,r)

def quickSortt(array,l,r):
    if l < r:
        m = partitionTestt(array,l,r)
        quickSortt(array,l,m-1)
        quickSortt(array,m+1,r)

def quickSorttt(array,l,r):
    if l < r:
        m = partitionTesttt(array,l,r)
        quickSorttt(array,l,m-1)
        quickSorttt(array,m+1,r)

def lomutoPartition(array,l,r):
    piv = randint(l,r)
    swap(array,piv,r)
    i = l

    for j in range(i,r):
        if array[j] < array[r]:
            swap(array,i,j)
            i=i+1
    swap(array,i,r)
    return i

def quickSortL(array,l,r):
    if l < r:
        m = lomutoPartition(array,l,r)
        quickSortL(array,l,m-1)
        quickSortL(array,m+1,r)

def hoarePartition(array,l,r):
    piv=int(floor((l+r)/2.0))
    i = l - 1; j = r + 1
    while 1:
        i+=1
        while array[i] < array[piv]:
            i+=1
        j-=1
        while array[j] > array[piv]:
            j-=1
        if i >= j:
            return j
        swap(array,i,j)

def quickSortH(array,l,r):
    if l < r:
        m = hoarePartition(array,l,r)
        quickSortH(array,l,m)
        quickSortH(array,m+1,r)


x = [randint(1,10000) for _ in range(10000)]
temp = []

temp=x.copy()

start = time.time_ns()
quickSortt(x,0,len(x)-1)
end = time.time_ns()
print("Window version:")
print(f"{str(end-start)} ns")

temp=x.copy()

start = time.time_ns()
quickSorttt(x,0,len(x)-1)
end = time.time_ns()
print("Better window version:")
print(f"{str(end-start)} ns")

x=temp.copy()

start = time.time_ns()
quickSort(x,0,len(x)-1)
end = time.time_ns()
print("Pushing version:")
print(f"{str(end-start)} ns")

x=temp.copy()

start = time.time_ns()
quickSortL(x,0,len(x)-1)
end = time.time_ns()
print("Lomuto version:")
print(f"{str(end-start)} ns")

x=temp.copy()

start = time.time_ns()
quickSortH(x,0,len(x)-1)
end = time.time_ns()
print("Hoare version:")
print(f"{str(end-start)} ns")