wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. I wrote this simple language in C# before reading books about interpreters. It's a bit odd as a result. The syntax is copied from Lisp because it was easy to parse and the metatypes are inspired by Lua. 

Overview
========
There existing types are bool, string, number, list, map, range and function. There are, however, two types of functions. Magic functions are passed syntax nodes rather than evaluated arguments. 
They can evaluate their arguments at their leisure. Strings in single quotes are not subject to interpolation. Strings in double quotes are interpolated. 

Types
=========
All types have a metatype. A metatype contains a methods that define how a value interacts with the default functions. You can define new or override existing functions in the metatype. For example, you can define the `Invoke` metamethod on a list value. This allows that list instance to be used like a function. Why? This greatly simplified the core interpreter.

Calling .NET Functions
======================
Use the `::` function to invoke a .NET method. 

Examples
=======
```lisp
(defn fact (a) 
  (if (< a 2) 
    (then 1)
    (else (* a (fact (- a 1))))
  )
)

(print "10! is {(fact 10)}.") ; prints '10! is 3628800.'
```

```lisp
(defn apply (func list)
  (if (empty? list) 
    (then list) 
    (else 
      (concat ; also known as ..
        ((func (first list))) 
        (apply func (rem list))
      )
    )
  )
)

(def +1 (lambda (a) (+ a 1))) ; +1 is a valid identifier not a number
(apply +1 (1 2 3)) ; returns (2 3 4)
```

```lisp
; Call .NET functions
(:: System.String.Join ' ' ('a' 'b' 'c')) ; returns ('a b c')
(:: System.Console.WriteLine "ok") 
(:: System.Console.Title "New title")
(:: System.Console.Title) 
(:: System.DateTime.Now) 

```

```lisp
; Maps
(defn make-person (name age) (@object (name age)))
(def me (make-person 'szensk' 26))
(def bill (make-person 'Bill' 72))
(= me bill) ; returns false
(def me2 (make-person 'szensk' 26))
(= me me2) ; returns true
(at me age) ; returns 26
(set me name 'New Name') ; modifies map inplace 
```

```lisp
; Ranges are specified by [start end increment]
; if you omit the increment it is assumed to be either 1 or -1
; Ranges can be used to generate lists
(def l (list [1 5])) 
(def size (- (# l) 1))
; ranges can be used as functions, in which case it is used as an index into a list
([size 0] l) ; returns l in backwards order, (5 4 3 2 1)
([0] l)      ; returns the first element of l, 1
([0 size 2] l) ; returns every other element of l, (1 3 5)
```

```lisp
; Metamethod fun

; By default the concat operator .. does not work on numbers
(.. 2 3) ; error, unable to call concat on type number 

(defn join-numbers (a b) (.. (string a) (string b)))

; Set the metamethod on a value
(set-metamethod 2 .. join-numbers)

(.. 2 3) ; returns '23'
(.. 3 2) ; error, unable to call concat on type number

; Set the metamethod on a type
(set-metamethod Number .. join-numbers)
(.. 3 2) ; returns '32'
```