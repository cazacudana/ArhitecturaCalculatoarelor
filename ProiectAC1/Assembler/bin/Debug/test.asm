CLR (R0)                                     
MOV R2,R3
MOV R1,(R2)5
ADD R1,1
Eticheta:
ADD (R3)2,R4
SUB (R5)15,R4
DEC R7
JMP (R1)36 
MOV R2,12
ADD R12,R9
POP R1
BNE Eticheta3
NOP
Eticheta2:
PUSH_PC
POP_PC
Eticheta3:
PUSH_FLAGS
BR Eticheta2
POP_FLAGS
ADD (R12)1,(R9)2
BR Eticheta3
BEQ Eticheta2
BVS Eticheta
BVC Eticheta2

