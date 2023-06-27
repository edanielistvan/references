#ifndef OPERATION_H_
#define OPERATION_H_

#include <string>
#include <vector>

enum Operations {PLUS, MINUS, MULTIPLY, DIVIDE, MINIMUM, MAXIMUM, MOD};
unsigned int max_len = 20;
unsigned int test_runs = 10;
unsigned int tests_num = 1000000;

class Operation {
    protected:
        Operations optor;
        unsigned int num;
    public:
        Operation() {
            this->num = rand();
            optor = Operations(rand() % (Operations::MOD+1));
        }

        Operation(const Operation& op) {
            this->num = op.getNum();
            optor = op.getOptor();
        }

        ~Operation() {}

        void mutate(int type) {
            if (type == 0 || type == 2) this->num = rand();
            if (type == 1 || type == 2) this->optor = Operations(rand() % (Operations::MOD+1));
        }

        Operations getOptor() const {
            return optor;
        }

        unsigned int getNum() const {
            return num;
        }

        virtual bool isTrivial() const {
            return true;
        }

        virtual unsigned int doOperation(unsigned int base) {
            return base;
        }
        virtual void mergeNum (unsigned int num) {}
};

class Plus : public Operation 
{
    public:
        Plus() {
            this->num = rand();
            optor = Operations::PLUS;
        }

        Plus(const Plus& op) {
            this->num = op.getNum();
            optor = Operations::PLUS;
        }

        ~Plus() {}

        unsigned int doOperation(unsigned int base) override {
            return base + num;
        }

        void mergeNum(unsigned int num) override {
            this->num += num;
        }

        bool isTrivial() const override {
            return num == 0;
        }
};

class Minus : public Operation 
{
    public:
        Minus() {
            this->num = rand();
            optor = Operations::MINUS;
        }

        Minus(const Minus& op) {
            this->num = op.getNum();
            optor = Operations::MINUS;
        }

        ~Minus() {}

        unsigned int doOperation(unsigned int base) override {
            return base - num;
        }

        void mergeNum(unsigned int num) override {
            this->num += num;
        }

        bool isTrivial() const override {
            return num == 0;
        }
};

class Times : public Operation 
{
    public:
        Times() {
            this->num = rand();
            optor = Operations::MULTIPLY;
        }

        Times(const Times& op) {
            this->num = op.getNum();
            optor = Operations::MULTIPLY;
        }

        ~Times() {}

        unsigned int doOperation(unsigned int base) override {
            return base * num;
        }

        void mergeNum(unsigned int num) override {
            this->num *= num;
        }

        bool isTrivial() const override {
            return num == 1;
        }
};

class Over : public Operation 
{
    public:
        Over() {
            this->num = rand();
            optor = Operations::DIVIDE;
        }

        Over(const Over& op) {
            this->num = op.getNum();
            optor = Operations::DIVIDE;
        }

        ~Over() {}

        unsigned int doOperation(unsigned int base) override {
            if (num == 0) return base;
            else return static_cast<unsigned int>(base / num);
        }

        void mergeNum(unsigned int num) override {
            this->num *= num;
        }

        bool isTrivial() const override {
            return num == 1 || num == 0;
        }
};

class Lesser : public Operation 
{
    public:
        Lesser() {
            this->num = rand();
            optor = Operations::MINIMUM;
        }

        Lesser(const Lesser& op) {
            this->num = op.getNum();
            optor = Operations::MINIMUM;
        }

        ~Lesser() {}

        unsigned int doOperation(unsigned int base) override {
            return base < num ? base : num;
        }

        void mergeNum(unsigned int num) override {
            this->num = (this->num < num ? this->num : num);
        }

        bool isTrivial() const override {
            return false;
        }
};

class Greater : public Operation 
{
    public:
        Greater() {
            this->num = rand();
            optor = Operations::MAXIMUM;
        }

        Greater(const Greater& op) {
            this->num = op.getNum();
            optor = Operations::MAXIMUM;
        }

        ~Greater() {}

        unsigned int doOperation(unsigned int base) override {
            return base > num ? base : num;
        }

        void mergeNum(unsigned int num) override {
            this->num = (this->num > num ? this->num : num);
        }

        bool isTrivial() const override {
            return false;
        }
};

class Modulo : public Operation 
{
    public:
        Modulo() {
            this->num = rand();
            optor = Operations::MOD;
        }

        Modulo(const Modulo& op) {
            this->num = op.getNum();
            optor = Operations::MOD;
        }

        ~Modulo() {}

        unsigned int doOperation(unsigned int base) override {
            if (num == 0) return base;
            else return base % num;
        }

        void mergeNum(unsigned int num) override {
            if(num <= this->num) this->num = num;
        }

        bool isTrivial() const override {
            return false;
        }
};

class OperationFactory {
    public:
        static Operation* createOperation(const Operations& opt) {
            if (opt == Operations::PLUS) return new Plus();
            else if (opt == Operations::MINUS) return new Minus();
            else if (opt == Operations::MULTIPLY) return new Times();
            else if (opt == Operations::DIVIDE) return new Over();
            else if (opt == Operations::MINIMUM) return new Lesser();
            else if (opt == Operations::MAXIMUM) return new Greater();
            else if (opt == Operations::MOD) return new Modulo();
            else return nullptr;
        }
};

class Creature {
    private:
        std::vector<Operation*> operations;
        std::vector<unsigned int> tests;

        unsigned int evaluate(unsigned int num) const {
            unsigned int cur = num;
            for(int i = 0; i < operations.size(); i++) {
                cur = operations[i]->doOperation(cur);
            }
            return cur;
        }

        void optimize() {
            std::cerr << "Unoptimized length: " << operations.size() << std::endl; 

            int i = 0;
            while(i < operations.size()) {                
                Operations type = operations[i]->getOptor();
                int j=i+1;

                while(j < operations.size() && operations[j]->getOptor() == type) {
                    operations[i]->mergeNum(operations[j]->getNum());
                    operations.erase(operations.begin() + j);
                }

                i++;
            }

            for(int i = 0; i < operations.size(); i++)
            {
                if (operations[i]->isTrivial()) {
                    operations.erase(operations.begin() + i);
                }
            }

            std::cerr << "Optimized length: " << operations.size() << std::endl;
        }

    public:
        Creature() {
            unsigned int len = rand() % max_len;
            if (len < 1) len = 1;

            operations.resize(len);
            for(unsigned int i = 0; i < len; i++) {
                Operations opt = Operations(rand() % (Operations::MOD+1));
                operations[i] = OperationFactory::createOperation(opt);
            }

            optimize();
        }

        Creature(const Creature& cr) {
            std::vector<Operation*> op = cr.getOperations();
            for(unsigned int i = 0; i < op.size(); i++) {
                operations.push_back(OperationFactory::createOperation(op[i]->getOptor()));
            }

            optimize();
        }

        ~Creature() {
            for(unsigned int i = 0; i < operations.size(); i++) {
                delete operations[i];
            }
        }

        std::vector<Operation*> getOperations() const {
            return operations;
        }

        double getAverage() const {
            double avg = 0;
            for(int i = 0; i < tests.size(); i++) {
                avg += static_cast<double>(tests[i]);
            }

            return avg / static_cast<double>(tests.size());
        }

        void operator()(Creature* op) {
            op->doTests();
        }

        int getLeng() const {
            return operations.size();
        }

        void mutation() {
            unsigned int siz = operations.size();

            unsigned int raise = rand() % max_len;
            int am = (rand() % 20) - 9;

            if(am > 0 && raise > operations.size()) {
                unsigned int size = operations.size() + am;
                if (size > max_len) size = max_len;

                operations.resize(size);

                for(int i = 0; i < siz; i++) {
                    int random = rand() % 100;
                    if(random > 50) operations[i]->mutate(rand() % 3);
                }

                for(int i = siz; i < size; i++) {
                    Operations opt = Operations(rand() % (Operations::MOD+1));
                    operations[i] = OperationFactory::createOperation(opt);
                }
            }
            else if(am < 0 && raise < operations.size()) {
                int size = operations.size() + am;
                if (size < 1) size = 1;

                operations.resize(size);

                for(int i = 0; i < size; i++) {
                    int random = rand() % 100;
                    if(random > 50) operations[i]->mutate(rand() % 3);
                }
            }
            else {
                for(unsigned int i = 0; i < operations.size(); i++) {
                    int random = rand() % 100;
                    if(random > 50) operations[i]->mutate(rand() % 3);
                }
            }

            optimize();
        }

        void doTests() {
            tests.clear();
            for(unsigned int i = 0; i < test_runs; i++) {
                std::vector<int> possibles;
                possibles.resize(tests_num);

                unsigned int start = rand();

                for(unsigned int j = 0; j < tests_num; j++) {
                    start = evaluate(start);
                    possibles[start % tests_num]++;
                }

                unsigned int error = 0;
                unsigned int target = tests_num / possibles.size();
                for(unsigned int j = 0; j < possibles.size(); j++) {
                    error += abs(static_cast<double>(target) -  static_cast<double>(possibles[j]));
                }

                tests.push_back(error);
            }
        }

        std::string getOpString() const {
            std::string v = "START";

            std::string s[] = {"+","-","*","/","min","max","%"};

            for(int i = 0; i < operations.size(); i++) {
                v += " " + s[operations[i]->getOptor()] + " " + std::to_string(operations[i]->getNum());
            }

            return v;
        }
};

#endif