#!/usr/bin/python3
import struct as st
import sys
import socket as sc
import select as sel
import math

port = 10000 if len(sys.argv) < 2 else int(sys.argv[1])
serverAddress = ('localhost',port)

unpacker = st.Struct('2i c')
packer = st.Struct('i')

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as server:
    inputs = [server] 

    server.bind(serverAddress)
    server.listen(1)
    server.settimeout(1.0)
    server.setsockopt(sc.SOL_SOCKET, sc.SO_REUSEPORT, 1)

    while True:
        read, write, excep = sel.select(inputs, inputs, inputs, 1)

        if not (read or write or excep):
            continue

        for socket in read:
            if socket is server:
                client, clientAddr = socket.accept()
                inputs.append(client)
                print("Kapcsolodott: ",clientAddr)
            else: 
                data = socket.recv(16)

                if not data:
                    print("Kilepett: ",socket.getpeername())
                    inputs.remove(socket)
                    socket.close()
                else:
                    mdata = unpacker.unpack(data)
                    print("Calculating: ",mdata)

                    operator = mdata[2].decode()

                    if operator == '+':
                        socket.sendall(packer.pack(mdata[0] + mdata[1]))
                    elif operator == '-':
                        socket.sendall(packer.pack(mdata[0] - mdata[1]))
                    elif operator == '*':
                        socket.sendall(packer.pack(mdata[0] * mdata[1]))
                    elif operator == '/':
                        socket.sendall(packer.pack(mdata[0] // mdata[1]))
                    else:
                        socket.sendall(packer.pack(0))

                    print("Done.")