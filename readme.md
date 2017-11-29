wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. I wrote this simple language in C# before reading books about interpreters. It's a bit odd as a result. The syntax is borrowed from Lisp for its parsing simplicity. The metatypes are inspired by Lua's metatables. 

Types
======
There are only a eight basic types: Bool, Number, String, List, Map, Range, Function, SyntaxNode and NetObject. Strings come in two varieties, interpolated (e.g. `"hello {world}.`) and regular (e.g. `'this is a bracket {}'`). Functions also exist in two varieties. Regular functions operate as you expect but the magic functions are passed SyntaxNodes instead of evaluated arguments. These syntax nodes may be evaluated using the `eval` function. 

All types have a metatype. A metatype contains methods that define how a value interacts with existing functions. You can define new or override existing functions on a metatype. For example, you can define the `invoke` metamethod on a map value. This allows that map instance to be used like a function. The first argument is the list itself. Using this it is possible construct an object-oriented programming system.

.NET Interoperability
=====================
The `::` function can invoke static .NET methods and the `new-object` function can construct new .NET objects. These objects are wrapped in a NetObject. Fields and properties of an object can be accessed with `(at obj PropertyName)` and set with `(set obj PropertyName 5)`. Methods on objects may be invoked by using the NetObject like a function, for example `(obj Create 'argument1')`.

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
; A normal function has its arguments evaluated left-to-right before it is executed
(defn ltr (a b) (?? a b))

; Magic functions receive unevaluated syntax nodes and may evaluate them in any order
(@defn rtl (a b) (?? (eval b) (eval a)))

(ltr (print "1") (print "2")) ; prints 1 then 2
(rtl (print "1") (print "2")) ; prints 2 then 1
```

See the examples directory for more.