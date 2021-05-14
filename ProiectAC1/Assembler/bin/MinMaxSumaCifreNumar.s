;**** WINDLX: Calculul minimului/maximului/sumei unui sir de numere ***
;-------------------------------------------------------------------------------------------
;Programul necesita subrutina INPUT.s
; Rezultatele sunt afisate pe consola
;-------------------------------------------------------------------------------------------
		.data
Prompt:
		.asciiz "Introduceti numarul de elemente al sirului "
Prompt1:
		.asciiz "Introduceti elementul = "
PrintfFormat1:
		.asciiz "Maximul este %d :\n"
PrintfFormat2:
		.asciiz "Minimul este %d :\n"
PrintfFormat3:
		.asciiz "Suma este %d :\n"
PrintfMesaj_err:
		.asciiz "\nNumar negativ! Reluati!\n"
		.align 2
PrintfPar1:
		.word PrintfFormat1 
PrintfValue1:
		.space 4 
PrintfPar2:
		.word PrintfFormat2 
PrintfValue2:
		.space 4 
PrintfPar3:
		.word PrintfFormat3 
PrintfValue3:
		.space 4 
PrintfErr1:
		.word PrintfMesaj_err
							
		.text
		.global main
		
		main:
				addi r1,r0,Prompt1 ; citire dela tastatura a numarului n 
				addi r8,r0,10 ;salvez valoarea 10 pentru detrminarea cifrelor numarului.
				add r7,r1,0  ;transfer inr7 numarul citit pentru a putea opera asupra lui
				addi r13,r0,0;acesta va fi contorul decifre 
	ExtragereCifre:
	                add r9,r7,r0 ; r7 primeste r9 salvare temporara anumarului ramas dupa eliminarea a cate o cifra
                    divu r9,r9,r8 ; r9 ¬ r9 impartit 10 catul împartirii
					multu r10,r9,r8 ; secventa necesara pentru preluarea inR10 pun catul*10
								  ; restului împartirii
					subu r1, r9, r8; r1 ¬ r9–[(r9/10)]*10 (restul
                                      ; împartirii – cifra curenta) 
					addi r13,r13,1  ;incrementez contorul decifre
					
					bnez r10,ExtragereCifre
					
					
				;jal InputUnsigned ; r1 = retine elementul current al sirului
				sle r15,r1,r0
				bnez r15, mesaj_err ; daca s-a tastat un numar negativ se afiseaza
									; mesaj urmat dereluarea introducerii
									; numarului dela tastatura
				addi r12,r13,0   ;salvez inR12 nr decifre
				addi r2,r12,0 ; r2 = preia numarul deelemente al sirului
				addi r3,r0,-32000 ; r3 = va stoca maximul
				addi r4,r0,32000 ; r4 = va stoca minimul
				addi r6,r0,0 ; r6 = suma
		
		
		loop:
				;addi r1,r0,Prompt1  începe citirea sirului element cu element
				jal ExtragereCifre; r1 = retine elementul current al sirului
				sle r15,r1,r0 ; daca s-a tastat un numar negativ se afiseaza
								; mesaj
				bnez r15, mesaj_err_el ; urmat dereluarea introducerii
										; numarului dela tastatura
				slt r5,r1,r4 ; daca r1>r4 atunci minimul va deveni ultimul
										; element citit
				beqz r5,maxim ; altfel se continua executia fireasca
							  ; comparându-se elementul curent cu maximul
							  ; din sir obtinut pâna în acest moment
				addi r4,r1,0 ; minimul devine ultimul element citit r4 ¬ r1
	    
		
		maxim:
				sgt r5,r1,r3 ; daca r1>r3 atunci maximul va deveni ultimul
							; element citit
				beqz r5,suma ; altfel se continua executia fireasca
							; calculându-se suma partiala a elementelor
							; din sir citite pâna în acest moment
				addi r3,r1,0 ;maximul devine ultimul element citit r3 ¬ r1
		suma:
				add r6,r6,r1 ; r6 ¬ r6 + r1
				subi r2,r2,1 ; r2 ¬ r2 –1: decrementare contor elemente
							; citite
				bnez r2,loop ; mai sunt elemente decitit dela tastatura ?
				sw PrintfValue1,r3 ; salvare parametrii afisare pentru maxim
				addi r14,r0,PrintfPar1
				trap 5 ; afisare maxim
				sw PrintfValue2,r4 ; salvare parametrii afisare pentru minim
				addi r14,r0,PrintfPar2
				trap 5 ; afisare minim
				sw PrintfValue3,r6 ; salvare parametrii afisare pentru suma
				addi r14,r0,PrintfPar3
				trap 5 ; afisare suma
				j gata
	mesaj_err:
				sw PrintfValue3,r6 ; salvare parametrii afisare pentru mesajde
								   ; eroare
				addi r14,r0,PrintfErr1
				trap 5 ; afisare mesaj eroare ‘ Dimensiune sir negativa !’
				j main ; reluare cu tastarea dimensiunii sirului
	mesaj_err_el:
				sw PrintfValue3,r6 ; salvare parametrii afisare pentru un al doilea
								   ; mesaj eroare
				addi r14,r0,PrintfErr1
				trap 5 ; afisare mesaj eroare ‘Element negativ!’
				j loop ; reluare cu tastarea elementului curent
	gata:
				trap 0 ; încheiere program