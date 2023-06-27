#include <stdio.h>
#include <stdlib.h>

#define true 1
#define false 0

enum type { NUMBER, OPERATOR, BRACKET, DEFAULT };
typedef enum type Type;

struct string {
    char* text;
    int length;
};

typedef struct string String;

struct unit {
    Type type; 
    String* value;
};

typedef struct unit Unit;

char operators[8] = {'+', '-', '*', '/', 'd', '(', ')', '\0'};
int precedence[8] = {0, 0, 1, 1, 2, 3, 3, -1};

int stringLength(char* string) {
    int i = 0;
    while (string[i] != '\0') i++;
    return i;
}

void stringCopy(char* from, char* to) {
    int length = stringLength(from);

    for (int i = 0; i <= length; i++) {
        to[i] = from[i];
    }
}

int isOperator(char operator) {
    int i = 0;
    int len = stringLength(operators);
    while (i < len && operators[i] != operator) i++;
    return i < len;
}

int opIndex(char operator) {
    int i = 0;
    int len = stringLength(operators);
    while (i < len && operators[i] != operator) i++;
    return (i < len) ? i : -1;
}

int isNumber(char n) {
    return (n >= '0' && n <= '9');
}

int isValid(char* buffer) {
    int length = stringLength(buffer);
    int open = 0;
    int close = 0;

    for (int i = 0; i < length; i++) {
        if (isOperator(buffer[i])) {
            if (i < length - 1 && isOperator(buffer[i+1]) && buffer[i] != ')' && buffer[i+1] != '(') {
                return false;
            }

            if (buffer[i] == '(') open++;
            else if (buffer[i] == ')') close++;
        }
        else if (!isNumber(buffer[i])) {
            return false;
        }
    }

    return open == close;
}

Unit* createUnits(char* buffer, int length, int* counter) {
    (*counter) = 0;
    int current = 10;

    Unit* units = (Unit*)malloc(sizeof(Unit) * current);
    int i = 0;

    while(i < length) {
        if (isNumber(buffer[i])) {
            units[(*counter)].type = NUMBER;
            units[(*counter)].value = (String*)malloc(sizeof(String));
            
            char temp[50]; temp[0] = buffer[i];
            int j = 1;

            while (i + j < length && isNumber(buffer[i+j])) {
                temp[j] = buffer[i+j];
                j++;
            }

            temp[j] = '\0';

            int len = stringLength(temp);      
            units[(*counter)].value->length = len;
            units[(*counter)].value->text = (char*)malloc(sizeof(char) * len);
            stringCopy(temp, units[(*counter)].value->text);

            i += j;
            (*counter)++;
        }
        else if (isOperator(buffer[i])) {
            if (buffer[i] == '(' || buffer[i] == ')') {
                units[(*counter)].type = BRACKET;
                units[(*counter)].value = (String*)malloc(sizeof(String));

                units[(*counter)].value->length = 1;
                units[(*counter)].value->text = (char*)malloc(sizeof(char) * 2);
                units[(*counter)].value->text[0] = buffer[i];
                units[(*counter)].value->text[1] = '\0';
            }
            else {
                units[(*counter)].type = OPERATOR;
                units[(*counter)].value = (String*)malloc(sizeof(String));

                units[(*counter)].value->length = 1;
                units[(*counter)].value->text = (char*)malloc(sizeof(char) * 2);
                units[(*counter)].value->text[0] = buffer[i];
                units[(*counter)].value->text[1] = '\0';
            }

            i++;
            (*counter)++;
        }

        if ((*counter) == current) {
            current += 10;
            units = (Unit*)realloc(units, sizeof(Unit) * current);
        }
    }

    return units;
}

void freeUnits(Unit* units, int* counter) {
    for(int i = 0; i < (*counter); i++) {
        free(units[i].value->text);
        free(units[i].value);
    }

    free(units);
}

Unit* convertToReversePolishNotation(Unit* units, int* counter) {
    Unit* output = (Unit*)malloc(sizeof(Unit) * (*counter));
    Unit* stack = (Unit*)malloc(sizeof(Unit) * (*counter));
    int current = 0;
    int stack_p = -1;

    for (int i = 0; i < (*counter); i++) {
        if (units[i].type == NUMBER) {
            output[current] = units[i];
            current++;
        }
        else if (units[i].type == OPERATOR) {
            int pred_c = precedence[opIndex(units[i].value->text[0])];
            
            while (stack_p >= 0 && 
                    stack[stack_p].type == OPERATOR && 
                    precedence[opIndex(stack[stack_p].value->text[0])] >= pred_c) {
                output[current] = stack[stack_p];
                stack_p--; current++;
            }

            stack_p++;
            stack[stack_p] = units[i];
        }
        else if (units[i].type == BRACKET) {
            if (units[i].value->text[0] == '(') {
                stack_p++;
                stack[stack_p] = units[i];
            }
            else {
                while (stack_p >= 0 && stack[stack_p].value->text[0] != '(') {
                    output[current] = stack[stack_p];
                    stack_p--; current++;
                }

                stack_p--; 
            }
        }
    }

    while (stack_p >= 0) {
        output[current] = stack[stack_p];
        stack_p--; current++;
    }

    for (int i = 0; i < (*counter); i++) {
        if (units[i].type == BRACKET) {
            free(units[i].value->text);
            free(units[i].value);
        }
    }

    free(units);
    free(stack);

    *counter = current;
    return output;
}

int main() {
    char buffer[500];

    while (true) {
        scanf("%s", buffer);

        int length = stringLength(buffer);
        int* counter = (int*)malloc(sizeof(int));
        char* text = (char*)malloc(sizeof(char) * length);
        stringCopy(buffer, text);

        if (isValid(text)) {
            Unit* units = createUnits(text, length, counter);
            units = convertToReversePolishNotation(units, counter);

            freeUnits(units, counter);
            free(counter);
        }

        free(text);
    }

    return 0;
}