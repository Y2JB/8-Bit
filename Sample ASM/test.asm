// Setup our data and our loop counters
MOV A 1
STR A 0x0A

MOV B 0x05
STR B 0x0C


// Count to 5
loop:

// Add 1 to A, store the result in memory and output the current value
LDR A 0x0A
MOV B 1
ADD
STR A 0x0A
OUT A

// Compare our A value with our loop/count value
CMP 0x0C

JNE loop:


HLT