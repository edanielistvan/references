#!/usr/bin/python3
import socket as sc
import select as sel
import sys

port = 10000 if len(sys.argv) < 2 else int(sys.argv[1])
serverAddress = ('localhost', port)

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as server:
    inputs = [server]

    server.setsockopt(sc.SOL_SOCKET, sc.SO_REUSEPORT, 1)
    server.bind(serverAddress)
    server.listen(1)

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
                data = socket.recv(200)

                if not data:
                    print("Kilepett: ",socket.getpeername())
                    inputs.remove(socket)
                    socket.close()
                else:
                    print("Kaptam: ",data.decode())
                    socket.sendall("OK".encode())