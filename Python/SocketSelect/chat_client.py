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

    name = input("Login: ")
    client.sendall(name.encode())
    print("Welcome to the chat.")

    while True:
        message = input("Message: ")

        if message == "exit":
            print("Logoff.")
            break
        elif message == "pass":
            pass
        else:
            client.sendall(message.encode())
        
        data = client.recv(530)

        if data:
            print(data.decode())      