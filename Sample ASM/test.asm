// Setup our data and our loop counters
MOV A 0
STR A 0x00

MOV B 10
STR B 0x01


// Count to 10
loop:

// Add 1 to A, store the result in memory and output the current value
LDR A 0x00
MOV B 1
ADD
STR A 0x00
OUT A

// Load our loop value and decrement it
LDR A 0x01
MOV B 1
SUB

// Save our loop counter
STR A 0x01

JNZ loop:


// TODO: Output the first 14 entires of the Fibonacci Sequence


HLT