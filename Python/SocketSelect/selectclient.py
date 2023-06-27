#!/usr/bin/python3
from http import server
import socket as sc
import select as sel
import sys

port = 10000 if len(sys.argv) < 2 else int(sys.argv[1])
serverAddress = ('localhost', port)

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as client:
    client.connect(serverAddress)

    client.sendall("Hello".encode())

    data = client.recv(200)

    if not data:
        print("No response.")
    else:
        print("Response: ",data.decode())