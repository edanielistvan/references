#include <array>
#include <string>
#include <vector>
#include <iostream>
#include <sstream>
#include <math.h>
#include <algorithm>

// Készítette: Bahil Botond, Hegedüs Armand Alex, Egyed Dániel István

struct Nbor {
    std::uint32_t y;
    std::uint32_t x;
    std::uint8_t tav;

    Nbor(std::uint32_t yb, std::uint32_t xb, std::uint8_t tavb)
    {
        y = yb;
        x = xb;
        tav = tavb;
    }
};

struct Area {
    unsigned int district; //kerület azonosító
    unsigned int infectionRate; //fertõzöttség mutató
    unsigned int infectionMinRate; //fertõzöttség mutató minimum idõben
    unsigned int healthRate; //beoltottság/gyógyultság mutató
    unsigned int population; //lakottsági mutató
    std::vector<Nbor> nbors; //szomszedok
};

struct Reader {
    //unsigned int data[3]; 
    std::array<unsigned int, 3> data; //game ID, Max tick ID, Country Count
    std::array<std::uint64_t, 4> factors; //4 random factor
    std::array<unsigned short, 2> dimension; //terület méret
    std::vector<std::vector<Area>> areas; //kerület = területek
    std::string message; //végén üzenet
    bool hasEnd; //vége
    bool needAnsw; //válaszra vár
};

std::uint64_t useFactor(std::uint64_t* factor);
std::uint32_t healing(unsigned int* curr_tick, unsigned int coord[], Reader& from);
void toTickInfo(Reader& from, unsigned int* curr_tick);
std::uint32_t infection(unsigned int curr_tick, unsigned int coord[], Reader& from);
int sameDistrict(unsigned int y1, unsigned int x1, unsigned int y2, unsigned int x2, Reader& from);

std::vector<std::vector<std::vector<Area>>> tick_info;

void toTickInfo(Reader& from, unsigned int* curr_tick)
{
    tick_info[*curr_tick] = from.areas;
}

std::uint64_t useFactor(std::uint64_t* factor)
{
    //egyenlet * factor
    //useFactor(factor) - helyett

    //egyenlet * useFactor(factor) - így használható
    std::uint64_t ertek = *factor;
    *factor = (*factor * 48271) % 0x7fffffff; //0x7fffffff = 2147483647
    return ertek;
}

int sameDistrict(unsigned int y1, unsigned int x1, unsigned int y2, unsigned int x2, Reader& from)
{
    return from.areas[y1][x1].district == from.areas[y2][x2].district ? 1 : 2;
}

std::vector<Nbor> neighbors(unsigned int coord[], Reader& from) //vissza: koordináták (y,x) vektorban
{
    std::vector<Nbor> nbors;
    nbors.push_back(Nbor(coord[0], coord[1], 0));

    //hiba esetén próbáld dimension számok cseréjét
    if (coord[0] == 0) {
        if (coord[1] == 0) {
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
        else if (coord[1] == from.dimension[1] - 1) {
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
        }
        else {
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
    }
    else if (coord[0] == from.dimension[0] - 1) {
        if (coord[1] == 0) {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
        else if (coord[1] == from.dimension[1] - 1) {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
        }
        else {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
    }
    else {
        if (coord[1] == 0) {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
        else if (coord[1] == from.dimension[1] - 1) {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
        }
        else {
            nbors.push_back(Nbor(coord[0] - 1, coord[1], sameDistrict(coord[0] - 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0] + 1, coord[1], sameDistrict(coord[0] + 1, coord[1], coord[0], coord[1], from)));
            nbors.push_back(Nbor(coord[0], coord[1] - 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] - 1, from)));
            nbors.push_back(Nbor(coord[0], coord[1] + 1, sameDistrict(coord[0], coord[1], coord[0], coord[1] + 1, from)));
        }
    }
    return nbors;
}

/* healing(curr_tick, coord) =>
-- valószínûleg nem > hanem <= vagy meg kell cserélni a kimeneteket (false, true) --
width + height > curr_tick ? floor(min(i = [1 .. width + height], tick_info[curr_tick - i, coord].infection_rate)
* factor1() % 10 / 20.0) : 0 */
std::uint32_t healing(unsigned int curr_tick, unsigned int coord[], Reader& from)
{
    return (uint64_t)from.dimension[0] + (uint64_t)from.dimension[1] < curr_tick ? floor(((from.areas[coord[0]][coord[1]].infectionMinRate * useFactor(&(from.factors[0]))) % 10) / 20.0) : 0;
}

// curr_tick > 0    
/*  infection(curr_tick, coord) => ceil(
        (
            avg (i = [1 .. mini(factor2() % 10 + 10, curr_tick)], infection(curr_tick - i, coord)) +
            sum (
                c = [coord, neighbours(coord)];
                t = factor3() % 7 + 3,
                tick_info[curr_tick-1, c].infection_rate >
                    (start_info[coord].district != start_info[c].district ?
                        2 :
                        coord != c ?
                            1 :
                            0
                    ) * t ?
                        clamp(start_info[coord].population - start_info[c].population, 0, 2) + 1 : 0
            )
        ) * (factor4() % 25 + 50) / 100.0
    ) */
std::uint32_t infection(unsigned int curr_tick, unsigned int coord[], Reader& from)
{
    if (curr_tick == 0) { return 0; }

    //avg kezd
    uint32_t fac = (useFactor(&(from.factors[1])) % 10) + 10;
    uint32_t mini = fmin(fac, (uint32_t)curr_tick);
    double avg = 0;

    for (int i = 1; i <= mini; i++)
    {
        avg += infection(curr_tick - i, coord, from);
    }

    avg /= (double)mini;
    //avg veg

    //sum kezd

    uint32_t t = (useFactor(&(from.factors[2])) % 7) + 3;
    for (int i = 0; i < from.areas[coord[0]][coord[1]].nbors.size(); i++)
    {     
        int y = from.areas[coord[0]][coord[1]].nbors[i].y;
        int x = from.areas[coord[0]][coord[1]].nbors[i].x;
        int tav = from.areas[coord[0]][coord[1]].nbors[i].tav;
        if (tick_info[curr_tick - 1][y][x].infectionRate > tav * t) avg += std::clamp((int)(tick_info[0][coord[0]][coord[1]].population - tick_info[0][y][x].population), 0, 2) + 1;
    }
    //sum veg

    avg *= ((useFactor(&(from.factors[3])) % 25) + 50) / 100.0;
    avg = ceil(avg);

    return (uint32_t)avg;
}

void readData(Reader& to) {
    std::string line;

    while (std::getline(std::cin, line)) {
        if (!line.rfind(".", 0))
            return;

        if (!line.rfind("WRONG", 0) ||
            !line.rfind("SUCCESS", 0) ||
            !line.rfind("FAILED", 0))
        {
            to.hasEnd = true;
            to.message = std::move(line);
        }
        else if (!line.rfind("REQ", 0)) {
            to.needAnsw = true;
            std::stringstream(std::move(line).substr(4)) >> to.data[0] >> to.data[1] >> to.data[2];
        }
        else if (!line.rfind("START", 0)) {
            to.needAnsw = false;
            std::stringstream(std::move(line).substr(6)) >> to.data[0] >> to.data[1] >> to.data[2];
        }
        else if (!line.rfind("FACTORS", 0)) {
            std::stringstream(std::move(line).substr(8)) >> to.factors[0] >> to.factors[1] >> to.factors[2] >> to.factors[3];
        }
        else if (!line.rfind("FIELDS", 0)) {
            std::stringstream(std::move(line).substr(7)) >> to.dimension[0] >> to.dimension[1];
            to.areas.resize(to.dimension[0], std::vector<Area>{to.dimension[1]});
            tick_info.resize(to.data[1], std::vector<std::vector<Area>>(to.dimension[0], std::vector<Area>(to.dimension[1])));
            /* tick_info.resize(to.data[1], std::vector<std::vector<Area>>{});
            for(int i = 0; i < to.data[1]; i++)
            {
                tick_info[i].resize(to.dimension[0],std::vector<Area>{to.dimension[1]});
            } */
        }
        else if (!line.rfind("FD", 0)) {
            std::stringstream ss(std::move(line).substr(3));
            std::size_t y, x;
            ss >> y >> x;
            Area& a = to.areas[y][x];
            ss >> a.district >> a.infectionRate >> a.population;
        }
        else {
            std::cerr << "READER ERROR HAPPENED: unrecognized command line: " << line << std::endl;
            to.hasEnd = true;
            return;
        }
    }
    std::cerr << "Unexpected input end." << std::endl;
    to.hasEnd = true;
}

int main() {
    char teamToken[] = "KNCrNG5D";
    // int seed = 0;

    std::cout << "START " << teamToken
        // << " " << seed 
        << std::endl << "." << std::endl;

    Reader reader = {};
    unsigned int curr_tick = 0;

    while (true) {
        readData(reader);

        if (reader.hasEnd)
            break;
        if (!reader.needAnsw)
            continue;

        std::cerr << "<FACTORS BEFORE GEN: " << reader.factors[0] << ", " << reader.factors[1] << ", " << reader.factors[2] << ", " << reader.factors[3] << ">" << std::endl;

        while (curr_tick < reader.data[1] || curr_tick == 0)
        {
            if (curr_tick == 0)
            {
                for (uint32_t i = 0; i < reader.dimension[0]; i++)
                {
                    for (uint32_t j = 0; j < reader.dimension[1]; j++)
                    {
                        unsigned int coord[] = { i,j };
                        reader.areas[i][j].nbors = neighbors(coord, reader);
                    }
                }

                toTickInfo(reader, &curr_tick);
            }

            curr_tick++;

            for (uint32_t i = 0; i < reader.dimension[0]; i++)
            {
                for (uint32_t j = 0; j < reader.dimension[1]; j++)
                {
                    unsigned int coord[] = { i,j };
                    std::uint32_t change = healing(curr_tick, coord, reader);
                    reader.areas[i][j].healthRate += change;
                    reader.areas[i][j].infectionRate -= change;
                    //reméljük nem gyógyulnak többen, mint amennyien betegek
                }
            }

            for (uint32_t i = 0; i < reader.dimension[0]; i++)
            {
                for (uint32_t j = 0; j < reader.dimension[1]; j++)
                {
                    unsigned int coord[] = { i,j };
                    std::uint32_t change = infection(curr_tick, coord, reader);
                    reader.areas[i][j].infectionRate = std::clamp((int)(change + reader.areas[i][j].infectionRate), 0, 100 - (int)reader.areas[i][j].healthRate);
                }
            }
        
            toTickInfo(reader, &curr_tick);
        }

        // Ha szeretnetek debug uzenetet kuldeni, akkor megtehetitek.
        // Maximalisan csak 1024 * 1024 bajtot!
        
        

        // standard out-ra meg mehet ki a megoldas! Mas ne irodjon ide ki ;)
        std::cout << "RES " << reader.data[0] << " " << reader.data[1] << " " << reader.data[2] << std::endl;
        for (auto& row : tick_info[reader.data[1]]) {
            for (auto& area : row)
                std::cout << area.infectionRate << " ";
            std::cout << std::endl;
        }
        std::cout << "." << std::endl;
    }
    std::cerr << "END (message): " << reader.message << std::endl;
}