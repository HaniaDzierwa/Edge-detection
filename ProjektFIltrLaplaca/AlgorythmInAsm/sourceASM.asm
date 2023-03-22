OPTION CASEMAP:NONE


; Hanna Dzierwa 
; AEI Inf gr1
; Filtr Laplace'a in asm

; name of the places in matrix 
;  0   top   0 
; left mid right
;  0   bot   0

; laplace'a matrix
;  0  -1   0 
; -1   4  -1
;  0  -1   0

.data

laplaceaMatrix word -1, -1, 4, -1, -1, 0, 0, 0
vertical word 0
horizontal word 3 
counter dword 0
zero word 255

.code

;parametry wejsciowe : 
;bitmapPointerIn - tablica z wartoœciami sk³adowymi pikseli obrazu
;bitmapPointerOut - tablica, gdzie wyliczone nowe wartoscsk³adowych pikseli sa zapisywane
;sizeElementsToProcess - ilosc elementów do przetworzenia
;heightImage*3 - trzykrotna wartosc wysokosci zdjecia 

;parametry wyjsciowe: 
;nic 

doAlgorythmInAsm proc 

; rcx - bitmapPointerIn, rdx - bitmapPointerOut; r8 - sizeElementsToProcess; r9- heightImage*3
; top: -heightImage*3, left: -3, mid: , right: +3, bot: +heightImage*3

mov r10d, counter     ; move counter to register r10

;heightImage*3
mov dword ptr[vertical],r9d    ; put heightImage*3 in variable 

 
;move matrix
movups xmm1, xmmword ptr[laplaceaMatrix] ; moves laplace'a matrix to xmm1

calculate:  ; start loop 
cmp r10,r8  ; check in counter is equals to size elements to process
je endFunc  ; if true jump to endFunc 

;top
mov r9w,vertical   ; move vertical value to register 
sub rcx,r9         ; substract vertical value from bitmapPointerIn to move bitmapPointer to top value 
mov eax, [rcx]     ; move value to eax 
pinsrb xmm0,eax,0b ; put value in xmm0 in the first two bytes
add rcx,r9         ; move back rcx to start-pointer 

;bot
add rcx,r9          ; add vertical value from bitmapPointerIn to move bitmapPointer to bot value 
mov eax, [rcx]      ; move value to eax 
pinsrb xmm0,eax,10b ; put value in xmm0 in the second two bytes
sub rcx,r9          ; move back rcx to start-pointer 


;mid 
mov eax, [rcx]         ; move value to eax 
pinsrb xmm0,eax,100b   ; put value in xmm0 in the third two bytes


;right
mov r9w, horizontal      ; move horizontal value to register 
add rcx,r9				 ; add vertical value from bitmapPointerIn to move bitmapPointer to right value 
mov eax, [rcx]		     ; move value to eax 
pinsrb xmm0,eax,110b	 ; put value in xmm0 in the thourth two bytes
sub rcx,r9  ; back	     ; move back rcx to start-pointer 

;left
sub rcx,r9					; sub vertical value from bitmapPointerIn to move bitmapPointer to left value 
mov eax, [rcx]				; move value to eax 
pinsrb xmm0,eax,1000b		; put value in xmm0 in the fifth two bytes
add rcx,r9  ;back			; move back rcx to start-pointer 

PMULLW xmm0, xmm1   ;  multiplied values from created list of value part of pixel and matrix-filter
PHADDW XMM0, XMM0  ; adding 2 values to each other (horizontally)
PHADDW XMM0, XMM0 
PHADDW XMM0, XMM0

PACKUSWB XMM0, XMM0  ; Pack signed words to unsigned bytes with saturation (range 0-255)

movd DWORD PTR [rdx],xmm0 ; put calculated value in outList 

inc rcx  ; bitmapPointerIn increment 
inc rdx  ; bitmapPointerOut increment
inc r10  ; counter increment 

jmp calculate  ; jump to calculate 

endFunc:      
ret  
doAlgorythmInAsm endp  ; end program
end 
