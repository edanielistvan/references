import socket as sc
import struct as st
import select
from random import randint
import sys

min_num = 1
max_num = 100

hostname = "localhost" if len(sys.argv) < 2 else sys.argv[1]
port = 10000 if len(sys.argv) < 3 else int(sys.argv[2])

server_address = (hostname, port)

packer = st.Struct("c i")

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as server:
    inputs = [server]

    random_num = randint(min_num, max_num)
    guessed = False

    server.bind(server_address)
    server.listen(1)
    server.settimeout(1.0)

    while True:
        read, write, excep = select.select(inputs, inputs, inputs, 1)

        if not (read or write or excep):
            if guessed:
                break
            else:
                continue

        for socket in read:
            if socket is server:
                client, clientAddr = socket.accept()
                inputs.append(client)

                print("Connected:", clientAddr)
            else:
                data = socket.recv(packer.size)

                if not data:
                    inputs.remove(socket)
                    socket.close()

                    print("Disconnected")
                else:
                    print("Got data")

                    if guessed:
                        socket.sendall(packer.pack('V'.encode(), 0))
                    else:
                        gdata = packer.unpack(data)
                        op = gdata[0].decode()

                        print("Checking", op)

                        if op == '<':
                            if random_num < gdata[1]:
                                print("Yes")
                                socket.sendall(packer.pack('I'.encode(), 0))
                            else:
                                print("No")
                                socket.sendall(packer.pack('N'.encode(), 0))
                        elif op == '>':
                            if random_num > gdata[1]:
                                print("Yes")
                                socket.sendall(packer.pack('I'.encode(), 0))
                            else:
                                print("No")
                                socket.sendall(packer.pack('N'.encode(), 0))
                        elif op == '=':
                            if random_num == gdata[1]:
                                print("Win")
                                guessed = True
                                socket.sendall(packer.pack('Y'.encode(), 0))
                            else:
                                print("Lose")
                                socket.sendall(packer.pack('K'.encode(), 0))
