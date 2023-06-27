import math

class Csucs:
    def __init__(self, nev: str):
        self.nev = nev
        self.csucsok = []
        self.d = math.inf
        self.pi = None

    def __str__(self):
        return self.nev

    def ugyanaz(self, masik: str) -> bool:
        return self.nev == masik

    def ujCsucs(self, masik, tav: int):
        self.csucsok.append((masik, tav))

def keresVaros(varos: str, varosok: list) -> int:
    i = 0
    n = len(varosok)
    while i < n and not varosok[i].ugyanaz(varos):
        i+=1
    return i

def hozzaadHaNincs(varos: str, varosok: list) -> int:
    index = keresVaros(varos, varosok)
    if index >= len(varosok):
        varosok.append(Csucs(varos))
    return index

varosok = []

def beolvas(varosok: list):
    fileNev = input("Input fajl neve: ")
    file = open(fileNev,"r") #csak olvas

    sorok = file.readlines() #osszes sor beolvasasa

    for sor in sorok:
        mostani = sor.split(";") #sor felbontasa
        index = hozzaadHaNincs(mostani[0].lower(), varosok) #letezik-e mar a varos, ha nem, akkor appendeljuk
        for i in range(1,len(mostani),2):
            csucs = hozzaadHaNincs(mostani[i].lower(), varosok)
            varosok[index].ujCsucs(varosok[csucs], int(mostani[i+1])) #csucs hozzaadasa + tavolsag

def kiir(varosok: list): #teszteleshez
    for varos in varosok:
        print(varos.nev + ":")
        for csucs in varos.csucsok:
            print(csucs[0].nev + " " + str(csucs[1]))

def minPrQ(varosok: list, s: int) -> list: #letrehozza a minimum priority queue-t, mint egy lista
    pri = []
    for i in range(0,len(varosok)):
        pri.append(varosok[i])
    pri.sort(key=lambda x: x.d) #rendezes d alapjan, novekvo sorrend -> min priority
    return pri

def adjust(q: list, c: Csucs): #a megvaltozott q[v] erteket beallitja es megfelelo helyre tolja
    i = 0
    while i < len(q) and c.nev != q[i].nev: i+=1
    temp = c
    while i > 0 and q[i-1].d > temp.d:
        q[i] = q[i-1]
        i-=1
    q[i] = temp

def remMin(q: list, varosok: list): #minimum kivetele
    s = q.pop(0)
    return keresVaros(s.nev, varosok)

def djikstra(varosok: list, ind: str, cel: str):
    indv = keresVaros(ind, varosok) #varos index kereses
    if indv >= len(varosok):
        print("Nincs ilyen indulo varos.")
        return
    
    celv = keresVaros(cel, varosok) #varos index kereses
    if celv >= len(varosok):
        print("Nincs ilyen cel varos.")
        return

    varosok[indv].d = 0 #kezdocsucs tavolsaga
    q = minPrQ(varosok, indv)
    u = indv #index, nem konkret csucs

    while varosok[u].d != math.inf and len(q) > 0:
        for csucs in varosok[u].csucsok:
            v = keresVaros(csucs[0].nev, varosok) #nev alapjan index kereses
            if(varosok[v].d > varosok[u].d + csucs[1]):
                varosok[v].pi = varosok[u]
                varosok[v].d = varosok[u].d + csucs[1]
                adjust(q,varosok[v])
        u = remMin(q,varosok) #itt indexet ad vissza

    print("A legrovidebb ut hossza: " + str(varosok[celv].d)) #hossz kiir

    ut = []
    c = varosok[celv] #az ut gyujtese celtol, visszafele
    while c != None: 
        ut.append(c.nev)
        c = c.pi

    ut.reverse() #a tomb megforditasa, hogy ind => cel legyen
    print("A legrovidebb ut: ")
    for u in ut: print(u.capitalize()) #kiiras

def main():
    varosok = []

    beolvas(varosok) #fajlbol beolvas
    #kiir(varosok)

    ind = input("Start telepules: ").lower()
    cel = input("Cel telepules: ").lower()

    djikstra(varosok, ind, cel) #algoritmus

main()