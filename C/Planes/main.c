#include <stdio.h>
#include <stdlib.h>
#include <signal.h>
#include <unistd.h>
#include <math.h>
#include <sys/wait.h>
#include <errno.h>
#include <sys/ipc.h>
#include <sys/msg.h>
#include <sys/types.h>
#include <sys/ipc.h>
#include <sys/shm.h>
#include <sys/sem.h>
#include <sys/stat.h>
#include <string.h>
#include <time.h>

#define CAPACITY 50

struct uzenet
{
    long mtype;
    int travel;
};

int main(int argc, char **argv)
{
    srand(time(NULL));

    pid_t charter;

    char *p;

    int num_people = strtol(argv[1], &p, 10);
    int status;

    int pipeCharter[2];
    int mqueue;

    int* mem;

    key_t key = ftok("RPNLPN", 420);
    mqueue = msgget(key, 0600 | IPC_CREAT);

    key = ftok("RPNLPN_FOCIAIR", 70);
    int shared = shmget(key, sizeof(int), IPC_CREAT|S_IRUSR|S_IWUSR);

    mem = shmat(shared, NULL, 0);

    if (mqueue < 0)
    {
        perror("Mqueue error.");
        exit(1);
    }

    if (pipe(pipeCharter) == -1)
    {
        perror("Errorr with charter pipe.");
        exit(1);
    }

    int start = num_people >= CAPACITY;

    charter = fork();

    if (charter < 0)
    {
        perror("Couldn't fork charter.");
        exit(1);
    }
    else if (charter == 0)
    {
        close(pipeCharter[1]);
        char buffer[500];

        read(pipeCharter[0], buffer, sizeof(buffer));

        if (strcmp(buffer, "NONE") == 0)
        {
            printf("Katar Air not needed.\n");
        }
        else
        {
            int num = strtol(buffer, &p, 10);
            printf("Katar Air is needed. Number of passengers: %d\n", num);

            int travel_time = rand() % 5 + 1; // 1-10 msec

            printf("Katar Air welcomes everyone on-board. There are %d passengers. We will arrive at home in %d time units.\n", num, travel_time);

            struct uzenet send;
            send.mtype = 1;
            send.travel = travel_time;

            msgsnd(mqueue, &send, sizeof(int), 0);

            sleep(travel_time);
        }

        close(pipeCharter[0]);
        shmdt(mem);
    }
    else
    {
        close(pipeCharter[0]);
        char buffer[500];

        int left = 0;

        if (num_people > 239)
        {
            left = num_people - 239;
            num_people = 239;
        }

        if (!start)
        {
            strcpy(buffer, "NONE");
        }
        else
        {
            sprintf(buffer, "%d", num_people);
        }

        write(pipeCharter[1], buffer, sizeof(buffer));
        printf("%s\n",argv[1]);

        if (start)
        {
            struct uzenet get;

            msgrcv(mqueue, &get, sizeof(int), 1, 0);

            printf("Katar Air took off, they will arrive in %d time units.\n", get.travel);
        }
        
        if (left > 0)
        {
            *mem = left;
        }

        close(pipeCharter[1]);
        msgctl(mqueue, IPC_RMID, NULL);
        shmdt(mem);
        shmctl(shared,IPC_RMID,NULL);

        waitpid(charter, &status, 0);
    }

    return 0;
}