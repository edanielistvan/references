#!/usr/bin/python3
import socket as sc
import sys
import struct as st

serverPort = 10000 if len(sys.argv) < 2 else int(sys.argv[1])

serverAddress = ('localhost', serverPort)

packer = st.Struct('2i c')
unpacker = st.Struct('i')
 
with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as client:
    client.connect(serverAddress)

    while True:
        op = input("Operator: ")

        if op == "exit":
            break

        fs = int(input("First: "))
        sn = int(input("Second: "))

        client.sendall(packer.pack(fs,sn,op.encode()))
        print("Requested to calc",fs,op,sn,".")
        
        data = client.recv(16)
        print("Received:",unpacker.unpack(data)[0])
