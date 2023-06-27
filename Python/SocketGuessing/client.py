import socket as sc
import struct as st
import sys

min_num = 1
max_num = 100

hostname = "localhost" if len(sys.argv) < 2 else sys.argv[1]
port = 10000 if len(sys.argv) < 3 else int(sys.argv[2])

server_address = (hostname, port)

packer = st.Struct("c i")

with sc.socket(sc.AF_INET, sc.SOCK_STREAM) as client:
    min_cur = min_num
    max_cur = max_num

    client.connect(server_address)

    while True:
        if max_cur != min_cur:
            client.sendall(packer.pack('<'.encode(), (min_cur + max_cur) // 2))

            data = client.recv(packer.size)

            if not data:
                break

            answer = packer.unpack(data)[0].decode()

            if answer in ['Y', 'V', 'K']:
                break
            elif answer == 'I':
                max_cur = (min_cur + max_cur) // 2
            else:
                client.sendall(packer.pack(
                    '>'.encode(), (min_cur + max_cur) // 2))

                data = packer.unpack(client.recv(packer.size))
                answer = data[0].decode()

                if answer in ['Y', 'V', 'K']:
                    break
                elif answer == 'I':
                    min_cur = (min_cur + max_cur) // 2
                else:
                    client.sendall(packer.pack(
                        '='.encode(), (min_cur + max_cur) // 2))
        else:
            client.sendall(packer.pack('='.encode(), (min_cur + max_cur) // 2))

            data = packer.unpack(client.recv(packer.size))
            answer = data[0].decode()

            if answer == 'Y':
                print("Win")
            elif answer == 'V':
                print("End")
            else:
                print("Lose")

            break  # answer doesn't matter, we disconnect
