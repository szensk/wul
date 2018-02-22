wul
========
A **w**orthless **u**nnecessary **l**anguage developed as a learning exercise. Written in C#, before I read about parsers and interpreters, it is a bit odd. While the simple syntax is borrowed from Lisp, it is not a proper lisp. Lua's metatables are the inspiration for wul's metatypes. 

Types
======
There are seven basic types: Bool, Number, String, List, Map, Range, Function. Strings come in two varieties, interpolated (e.g. `"hello {world}."`) and regular (e.g. `'this will appear exactly {as seen here}'`). There are also two types of functions. Regular functions operate as you expect but macro functions are passed unevaluated syntax nodes instead of value arguments and can return quoted syntax. These syntax nodes may be evaluated using the `eval` function. This makes it easy to implementing short circuit operators or control flow (see [unpack.wul](Examples/while.wul)).

MetaTypes
=========
A metatype contains methods that define how a value interacts with existing functions. Most values, except the value nil, have a metatype. By default, values are constructed with their type's metatype. You can define new or override existing functions on a metatype at either the value or type level. For example, you can define the `invoke` metamethod on a map value. This allows that map instance to be used like a function. The map itself may be accessed in the variable `self`. Using these properties, it is possible construct an object-oriented programming system (see [unpack.wul](Examples/unpack.wul)). Furthermore, this makes recursive unnamed functions very simple: `(self arguments)`. 

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
(defn ltr (a b) (?? a b))
(defmacro rtl (a b) (`?? b a))

(ltr (print "1") (print "2")) ; prints 1 then 2
(rtl (print "1") (print "2")) ; prints 2 then 1
```

See the [examples directory](Examples) for more.
