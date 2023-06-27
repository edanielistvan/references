import random as rnd
from math import floor, ceil

def binarySearch(arr,l,r,k):
    if l < r:
        m = floor((l + r) / 2)
        if arr[m] == k:
            return m
        elif arr[m] > k:
            return binarySearch(arr,l,m-1,k)
        else:
            return binarySearch(arr,m+1,r,k)
    else:
        while l >= 0 and arr[l] > k: l-=1
        while l < len(arr) and arr[l] < k: l+=1
        return l

def floorPowerOfTwo(x):
    t = 1
    while(2*t<=x):
        t*=2
    return t

def merge(arr, lis):
    n = []
    i, j = 0, 0
    while i < len(arr) and j < len(lis):
        if arr[i] < lis[j]:
            n.append(arr[i]); i+=1
        elif arr[i] > lis[j]:
            n.append(lis[j]); j+=1
        else:
            n.extend((arr[i], lis[j]))
            i+=1
            j+=1
    while i < len(arr):
        n.append(arr[i]); i+=1
    while j < len(lis):
        n.append(lis[j]); j+=1
    return n

def inPlaceMerge(arr, al, ar, bl, br):
    tmp = []
    for i in range(al,ar+1):
        tmp.append(arr[i])
    i,j,l = 0,bl,al
    while i < len(tmp) and j <= br:
        if tmp[i] < arr[j]:
            arr[l] = tmp[i]; i+=1; l+=1
        elif tmp[i] > arr[j]:
            arr[l] = arr[j]; j+=1; l+=1
        else:
           arr[l] = tmp[i]; i+=1; l+=1
           arr[l] = arr[j]; j+=1; l+=1
    while i < len(tmp):
        arr[l] = tmp[i]; i+=1; l+=1
    while j <= br:
        arr[l] = arr[j]; j+=1; l+=1


def fill(n):
    return list(range(1,n+1))

def swap(arr, i, j):
    s = arr[i]
    arr[i] = arr[j]
    arr[j] = s

def copy(frm, into):
    into.clear()
    for i in frm:
        into.append(i)

def reverse(arr,i,j):
    mid = int(ceil((i + j) / 2))
    for a in range(i,mid):
        swap(arr,a,j-(a-i))

def rotate(arr, amount, i, j):
    reverse(arr,i,j)
    reverse(arr,i,i+amount)
    reverse(arr,i+amount+1,j)

def mix(arr):
    i = len(arr) * 100
    while i > 0:
        x = rnd.randint(0,len(arr) - 1)
        y = rnd.randint(0,len(arr) - 1)
        if x != y:
            i -= 1
            swap(arr,x,y)

def simpleSwapSort(arr):
    n = len(arr)
    for i in range(n-1):
        for j in range(i+1,n):
            if arr[i] > arr[j]:
                swap(arr,i,j)

def minSort(arr):
    n = len(arr)
    for i in range(n-1):
        min_i = i
        for j in range(i+1,n):
            if arr[min_i] > arr[j]:
                min_i = j
        if min_i != i: 
            swap(arr,min_i,i)

def maxSort(arr):
    n = len(arr)
    for i in range(n-1,0,-1):
        max_i = i
        for j in range(i-1):
            if arr[max_i] < arr[j]:
                max_i = j
        if max_i != i: 
            swap(arr, max_i, i)

def pileification(arr):
    piles = [[arr[0]]]
    i = 1

    while i < len(arr):
        j = len(piles)-1
        l = len(piles)-1
        while j > 0 and piles[j][0] > arr[i]:
            j-=1
        while l > 0 and piles[l][len(piles[l])-1] < arr[i]:
            l-=1

        if arr[i] <= piles[j][0]:
            piles[j].insert(0,arr[i])
        elif j != len(piles)-1 and piles[j][0] < arr[i]:
            piles[j+1].insert(0,arr[i])
        elif arr[i] >= piles[l][len(piles[l])-1]:
            piles[l].append(arr[i])
        elif l != len(piles)-1 and piles[l][len(piles[l])-1] > arr[i]:
            piles[l+1].append(arr[i])
        else:
            piles.append([arr[i]])
        i+=1
    return piles

def unShuffleSort(arr):
    piles = pileification(arr)

    i = 0
    while len(piles) > 0:
        arr[i] = piles[0].pop(0)
        if len(piles[0]) == 0:
            piles.pop(0)
            if len(piles) == 0:
                break
        elif len(piles) > 1:           
            if piles[0][0] > piles[1][0]:
                s = piles.pop(0)
                index = binarySearch([i[0] for i in piles],0,len(piles)-1,s[0])
                piles.insert(index,s)
        i+=1

def hoarePartition(arr,l,r):
    piv=arr[int(floor((l+r)/2.0))]
    i = l - 1; j = r + 1
    while 1:
        i+=1
        while arr[i] < piv:
            i+=1
        j-=1
        while arr[j] > piv:
            j-=1
        if i >= j:
            return j
        swap(arr,i,j)

def quickSort(array,l,r):
    if l < r:
        p = hoarePartition(array,l,r)
        quickSort(array,l,p)
        quickSort(array,p+1,r)
 
def insertionSort(arr):
    for i in range(1,len(arr)):
        if arr[i] < arr[i-1]:
            a = arr[i]; j = i-1
            while j >= 0 and arr[j] > a:
                arr[j+1] = arr[j]
                j-=1
            arr[j+1] = a

def bubbleSort(arr):
    i=0; sw=True; n=len(arr)
    while i < n and sw:
        sw = False; u = n-1; j = n-1
        while j > i:
            if arr[j] < arr[j-1]:
                swap(arr,j,j-1)
                sw = True
                u = j
            j-=1
        i = u

def cycleSort(arr):
    for i in range(len(arr)):
        item = arr[i]
        pos = -1

        while pos != i:
            pos = i

            for j in range(pos + 1, len(arr)):
                if arr[j] <= item:
                    pos += 1

            arr[pos], item = item, arr[pos]

def cocktailShakerSort(arr):
    sw=True; n=len(arr); l = 0; r = n-1
    while sw:
        sw = False; i = l; u = 0
        while i < r:
            if arr[i] > arr[i+1]:
                swap(arr,i,i+1)
                u = i
                sw = True
            i+=1
        r = u
        if not sw:
            break
        sw = False; i = r-1; u = n-2
        while i >= l:
            if arr[i] > arr[i+1]:
                swap(arr,i,i+1)
                u = i
                sw = True
            i-=1
        l = u

def combSort(arr,shrink=1.24733):
    n = len(arr)
    gap = n
    sw = True
    
    while sw:
        sw = False
        gap = max(int(gap / shrink), 1)
        for i in range(n - gap):
            j = gap+i
            if arr[i] > arr[j]:
                swap(arr,i,j)
                sw = True

def strandSort(arr):
    sol = []

    while len(arr) > 0:
        sub = [arr.pop(0)]
        i = 0
        while i < len(arr):
            if arr[i] > sub[-1]:
                sub.append(arr.pop(i))
            i+=1
        sol = merge(sub,sol)
    copy(sol,arr)

x = fill(500)
mix(x)
t = []

def gnomeSort(arr):
    for i in range(1,len(arr)):
        pos = i
        while pos > 0 and arr[pos] < arr[pos-1]:
            swap(arr,pos,pos-1)
            pos-=1

def oddEvenSort(arr):
    sw = True
    while sw:
        sw = False
        for i in range(1,len(arr),2):
            if arr[i] > arr[i+1]:
                swap(arr,i,i+1)
                sw = True
        for i in range(0,len(arr),2):
            if arr[i] > arr[i+1]:
                swap(arr,i,i+1)
                sw = True

def shellSort(arr):
    gap = [701,301,132,57,23,10,4,1]; n = len(arr)
    for g in gap:
        for i in range(g,n):
            temp = arr[i]; j = i
            while j >= g and arr[j-g] > temp:
                arr[j] = arr[j-g]
                j-=g
            arr[j] = temp

def mergeSort(arr,l,r):
    if l < r:
        m = int(floor((l+r)/2))
        mergeSort(arr,l,m)
        mergeSort(arr,m+1,r)
        inPlaceMerge(arr,l,m,m+1,r)

def blockInsertion(arr,l,r):
    for i in range(l,r):
        if arr[i] < arr[i-1]:
            a = arr[i]; j = i-1
            while j >= 0 and arr[j] > a:
                arr[j+1] = arr[j]
                j-=1
            arr[j+1] = a

def blockSort(arr):
    pot=floorPowerOfTwo(len(arr))
    scale=len(arr)/pot

    for merge in range(0,pot,16):
        start = merge * scale
        end = start + 16 * scale
        blockInsertion(arr,start,end)

def maxInsertionSort(arr):
    i = len(arr) - 2
    while(i >= 0):
        if(arr[i] > arr[i+1]):
            a = arr[i]; j=i+1
            while j < len(arr) and arr[j] < a:
                arr[j-1] = arr[j]
                j+=1
            arr[j-1]=a 
        i-=1

copy(x,t)
blockSort(t)
print(t)