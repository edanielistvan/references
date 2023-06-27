#!/usr/bin/python3
import sys
import socket as sc
import select as sel

port = 10000 if len(sys.argv) < 2 else int(sys.argv[1])
serverAddress = ('localhost',port)

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as server:
    inputs = [server] 

    users = {}

    server.bind(serverAddress)
    server.listen(1)
    server.settimeout(1.0)
    server.setsockopt(sc.SOL_SOCKET, sc.SO_REUSEPORT, 1)

    print("Chat started.")

    while True:
        read, write, excep = sel.select(inputs, inputs, inputs, 1)

        if not (read or write or excep):
            continue

        for socket in read:
            if socket is server:
                client, clientAddr = socket.accept()
                inputs.append(client)

                data = client.recv(50)

                if not data:
                    print("Invalid login -> no username.")
                else:
                    users[clientAddr] = data.decode()
            else:
                data = socket.recv(200)

                if not data:
                    user = users.pop(socket.getpeername())

                    print("Logoff: ",user)
                    
                    inputs.remove(socket)
                    socket.close()
                else:
                    message = data.decode()
                    print("Message received, sending to all.")

                    for c in write:
                        if not (c is server or c is socket):
                            c.sendall(f"[{users[socket.getpeername()]}] {message}".encode())

                    print("Done.")