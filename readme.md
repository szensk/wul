wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. I wrote this simple lisp-like language without reading about interpreters. It's very bad as a result, but I learned more this way.

Examples
=======
```lisp
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

```lisp
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

(inc (1 2 3)) ; returns (2 3 4)
```

```lisp
(def apply (func list)
  (if (empty? list) 
    (then ()) 
    (else 
      (concat 
        ((func (first list))) 
        (apply func (rem list))
      )
    )
  )
)

(let +1 (lambda (a) (+ a 1)))
(apply +1 (1 2 3)) ; returns (2 3 4)
```