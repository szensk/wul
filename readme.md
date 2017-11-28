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

See the examples directory for more.