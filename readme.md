wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. I wrote this simple lisp-like language without reading about interpreters. It's very bad as a result, but I learned more this way.

Examples
=======
```
(def fact (a) 
  (
    (if (< a 2) 
      (then 1)
      (else (* a (fact (- a 1))))
    )
  )
)

(print (fact 10)) ; prints 3628800
```

```
(def inc (a) 
  (if (empty? a) 
    (then ()) 
    (else 
      (concat 
        ((+ (first a) 1)) 
        (inc (rem a))
      )
    )
  )
)

(inc (1 2 3)) ; prints (2 3 4)
```