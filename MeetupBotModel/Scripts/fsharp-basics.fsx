// Values and Functions
///////////////////////////////////////////////////////////////////////////////

// The Unit type represents the absence of a specific value.
// It has a single value represented by ().
let unit_value = ()

// A function with type (unit -> string)
let f () = "Hello World"

f     // this is a function value
f ()  // function application -- must pass the unit value to 0-argument function

// A function that is only called for side-effects returns the Unit type.
// Type is (unit -> unit) because the return type of printfn is Unit.
let sayHello () = printfn "Hello World"

// A single argument function
// The type of this function is inferred to be (int -> int)
let addOne x = x + 1

// Calling a function with arguments
addOne 10

// A two argument function
// Type is inferred to be (int -> int -> int) 
let add x y = x + y

// Calling a function with multiple arguments
add 1 2
add 1 (addOne 10) // must add parens when function argument calls another function
// add (1, 2)     // This is a compile error!! (1, 2) creates a tuple as we will see.

// Explicitly specifying argument types
// The type of this function is (float -> float -> float)
let addFloat (x:float) (y:float) = x + y

// Expressions
// In F#, everything is an expression, and an expression has a type that
// represents the type of value it will evaluate to.
// The return type of a function, is simply the type of the expression used
// for the function body.

// The type of this function is (int -> string)
let getSign x =
    if x > 0 then "Positive"   // The type of the if expression is 'string'
    else if x = 0 then "Zero"
    else "Negative"

// Lambdas or anonymous functions are defined with the 'fun' keyword
List.filter (fun i -> i > 0) [-1; -10; 0; 3; -3; 15]

// Function composition
let square x = x * x
let negate x = -x
let print  x = printfn "The value is %d" x

print( negate( square 4 ))

// Function composition with the >> operator
let squareNegatePrint = 
    square >> negate >> print

squareNegatePrint 5

// Function composition with the forward pipe operator '|>'
10 |> square |> negate |> print

// Types
///////////////////////////////////////////////////////////////////////////////

// Tuple
// Discriminated Union
// Record
// list (immutable)
// option

// Pattern matching
///////////////////////////////////////////////////////////////////////////////


// Offside rule
///////////////////////////////////////////////////////////////////////////////
