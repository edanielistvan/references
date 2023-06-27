#include <iostream>
#include <ctime>
#include "time.hpp"
#include <vector>
#include <string>
using namespace std;

//Compiler version g++ 6.3.0

bool sorted(vector<int> a, int n) {
  int i = 1;
  while(i < n && a[i-1] <= a[i]) i++;
  return i >= n;
}

string tellSorted(vector<int> a, int n) {
  string v;
  if(sorted(a,n)) v="The array is sorted!";
  else v="The array is not sorted";
  return v;
}

time_t currTime() {
  struct timeval time_now{};
  gettimeofday(&time_now, nullptr);
  time_t msecs_time = (time_now.tv_sec * 1000) + (time_now.tv_usec / 1000);
  return msecs_time;
}

void writeTime(time_t t1, time_t t2) {
  cout << t2-t1 << " ms" << endl;
}

void copy(const vector<int> from, vector<int> &to) {
  to.resize(from.size());
  for(int i = 0; i < from.size(); i++) {
    to[i] = from[i];
  }
}

void swap(vector<int> &a, int i, int j) {
  int s = a[i];
  a[i] = a[j];
  a[j] = s;
}

void exchangeSort(vector<int> &a, int n) {
  for(int i = 0; i < n-1; i++) {
    for(int j = i+1; j < n; j++) {
      if(a[i] > a[j]) swap(a,i,j);
    }
  }
}

void hybridExchangeSort(vector<int> &a, int n, int i) {
  if(i < n-1) {
    for(int j = i+1; j < n; j++) {
      if(a[i] > a[j]) swap(a,i,j);
    }
    
    hybridExchangeSort(a,n,i+1);
  }
}


void recursiveHelp(vector<int> &a, int n, int i, int j) {
  if(j < n) {
    if(a[i] > a[j]) swap(a,i,j);
    recursiveHelp(a,n,i,j+1);
  }
}
void recursiveExchangeSort(vector<int> &a, int n, int i) {
  if(i < n-1) {
    recursiveHelp(a, n, i, i+1);
    recursiveExchangeSort(a,n,i+1);
  }
}

int main()
{
    time_t t1, t2;
    int n = 10000; int max = 100000;
    srand(time(NULL));
    vector<int> array;
    array.resize(n);
    
    for(int i = 0; i < n; i++) {
      array[i] = (rand() % max) + 1;
    }
    
    vector<int> temp;
    
    cout << "Iterative" << endl;
    copy(array, temp);  
    t1=currTime();
    exchangeSort(temp, n);
    t2=currTime();
    cout << tellSorted(temp, n) << endl;
    writeTime(t1,t2);
    
    cout << "Hybrid" << endl;
    copy(array, temp);  
    t1=currTime();
    hybridExchangeSort(temp, n, 0);
    t2=currTime();
    cout << tellSorted(temp, n) << endl;
    writeTime(t1,t2);
    
    cout << "Recursive" << endl;
    copy(array, temp);  
    t1=currTime();
    recursiveExchangeSort(temp, n, 0);
    t2=currTime();
    cout << tellSorted(temp, n) << endl;
    writeTime(t1,t2);
}