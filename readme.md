wul
========
**W**ul is another **u**nnecessary **l**anguage developed as a learning exercise. The simple syntax is borrowed from Lisp but the language isn't exactly a lisp. It is influenced by Lua: function parity is flexible, functions often return multiple values rather than lists and a value's metatype defines its behavior with standard operators (akin to Lua's metatables). Too few arguments is equivalent to passing nil to all the remaining arguments. Extra arguments not specified as a formal parameter of the function become accessible via a list named `$args`. Variable arguments are supported with `...`. Named parameters are also supported: `(add y: 6 x: 5)` and `(add 5 6)` are equivalent.

Types
======
There are seven basic types: Bool, Number, String, List, Map, Range, Function. Strings come in two varieties, interpolated (e.g. `"hello {world}."`) and regular (e.g. `'this will appear exactly {as seen here}'`). There are also two types of functions. Regular functions operate as you expect but macro functions are passed unevaluated syntax nodes instead of value arguments and can return quoted syntax. These syntax nodes may be evaluated using the `eval` function. This makes it easy to implementing short circuit operators or control flow (see [while.wul](Examples/while.wul)). Ranges are enumerators of a sequence of values (e.g. `[0 10 2]` enumerates all values from 0 to 10 by increments of 2). Beside generating sequences, ranges can be used to index lists (e.g. `([0] (1 2 3))` grabs the first element of the list and `([2 0] (1 2 3))` returns the list reversed).

MetaTypes
=========
A metatype contains methods that define how a value interacts with existing functions. All values, except the value nil, have a metatype. By default, values are constructed with their type's metatype. You can define new or override existing functions on a metatype at either the value or type level. Since all simple values have a default invoke metamethod that builds lists, it is not necessary to use the `list` method to construct a list. `(1 2 3)` will suffice. As an example of user metamethods, you can define the `invoke` metamethod on a map value. The map itself may be used like a function and accessed in metamethods as the variable `self` (see [invoke.wul](Examples/invoke.wul) and [unpack.wul](Examples/unpack.wul)). Furthermore, an unnamed function may be accessed as the variable self and called recursively (e.g. invoking `(lambda (arg) (self arg))` results an infinite loop).

.NET Interoperability
=====================
The `::` function invokes static .NET methods and the `new-object` function constructs new .NET objects. These objects can be bound to name or stored in a list for later use. Fields and properties of an object can be accessed with `(at obj PropertyName)` and set with `(set obj PropertyName 5)`. Methods on objects may be invoked by using the value like a function, for example `(obj Join 'argument1')`.

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
(defn ltr (a b) (print 'ltr'))
(defmacro rtl (a b) (`do (print 'rtl') b a))

;arguments are evaluated before being passed to functions
(ltr (print "1") (print "2")) ; prints 1, 2, ltr 
;but arguments are not evaluated before being passed to macros
(rtl (print "1") (print "2")) ; prints rtl, 2, 1 
```

```lisp
; functions may return multiple values
(defn n-and-its-square (n) (values n (* n n)))
(print (n-and-its-square 5)) ; prints 5, 25 
(+ (n-and-its-square 7)) ; returns 56
```

See the [examples directory](Examples) for more.
