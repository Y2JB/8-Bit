// Setup our data and our loop counters
// Loop counter 
MOV A 0
STR A 0x0A

// What are we counting to?
MOV B 0x0A
STR B 0x0C


// Count to 10
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