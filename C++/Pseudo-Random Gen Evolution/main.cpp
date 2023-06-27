#include <iostream>
#include <time.h>
#include "operation.h"

unsigned int creature_num = 500;

int main() {
    srand(time(NULL));
    std::vector<Creature*> crs;
    
    crs.resize(creature_num);

    for(unsigned int i = 0; i < creature_num; i++) {
        crs[i] = new Creature();
    }

    std::string input;

    do
    {
        for(unsigned int i = 0; i < creature_num; i++) {
            
            std::cout << "Testing creature " << (i+1) << "..." << std::endl; 
            crs[i]->doTests();
            std::cout << "Creature " << (i+1) << " done. -> " << crs[i]->getAverage() << std::endl;
        }

        for(unsigned int i = 0; i < creature_num - 1; i++) {
            unsigned int min = i;
            for(unsigned int j = i+1; j < creature_num; j++) {
                if(crs[min]->getAverage() > crs[j]->getAverage()) min = j;
            }
            if(min != i) {
                Creature* s = crs[min];
                crs[min] = crs[i];
                crs[i] = s;
            }
        }

        std::cout << "Best creature: Length = " << crs[0]->getLeng() << " | Average = " << crs[0]->getAverage() << std::endl;
        std::cout << "Best method: " << crs[0]->getOpString() << std::endl;
        std::cin >> input;

        for(unsigned int i = 1; i < creature_num; i++) {
            delete crs[i];
            crs[i] = new Creature(*crs[0]);
            crs[i]->mutation();
        }
    } while (input.compare("x") != 0);
    

    return 0;
}