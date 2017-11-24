wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. I wrote this simple lisp-like language without reading about interpreters. It's very bad as a result but I learned a lot.

Overview
========
There are only bools, strings, numbers, lists and functions. There are, however, two types of functions. Magic functions are passed syntax nodes rather than evaluated arguments. 
They can evaluate their arguments at their leisure. Strings in single quotes are not subject to interpolation. Strings in double quotes are interpolated. 

Examples
=======
```lisp
(def fact (a) 
  (if (< a 2) 
    (then 1)
    (else (* a (fact (- a 1))))
  )
)

(print "10! is {(fact 10)}.") ; prints '10! is 3628800.'
```

```lisp
(def apply (func list)
  (if (empty? list) 
    (then ()) 
    (else 
      (.. ; aka concat function
        ((func (first list))) 
        (apply func (rem list))
      )
    )
  )
)

(let +1 (lambda (a) (+ a 1)))
(apply +1 (1 2 3)) ; returns (2 3 4)
```