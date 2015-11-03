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

// Tuple ("product" type)
// Combination of two or more values
// Values do not have to have the same type

// Representing a point (x,y) as a tuple
let point1 = (5,2)   // Type is (int * int)

// Functions to access the first and second elements of a tuple
fst point1  // 5
snd point1  // 2

// Tuples with different data types
// Type is (string * string * int)
let opus = ("Opus", "Dog", 12)
let tyrone = ("Tyrone", "Cat", 16)



// Discriminated Union ("sum" type) and Option
type Shape =
    | Rectangle of width : float * length : float
    | Circle of radius : float

let rect = Rectangle(width = 10.0, length = 5.0)
let circle = Circle(2.0)

// Option is a generic, discriminated union type provided by
// the core library.

// type Option<'a> =        // use a generic definition
//   | Some of 'a           // valid value
//   | None                 // missing

// Type is (int -> int option)
let keepIfPositive x = if x > 0 then Some x else None

keepIfPositive 3   // Some 3
keepIfPositive 0   // None



// Records
type Point = { x : float; y: float; z: float; }

let point2 = { x = 1.0; y=2.0; z=0.0 }
printfn "The point is at (%f,%f,%f)" point2.x point2.y point2.z

let point3 = { point2 with z = 10.0 }

// Record fields are immutable by default
type Mutable2DPoint = { mutable x: float; mutable y: float }



// list (immutable)
let aList = [ 1; 2; 3; ]
List.head aList   // 1
List.tail aList   // [ 2; 3; ]

// List constructor
let listWithZero = 0 :: aList



// array (mutable)
let array = [| 1; 2; 3; |]
array.[0] = 10  // Not assignment -- equality test instead!
array.[0] <- 10

// Pattern matching
///////////////////////////////////////////////////////////////////////////////
open System

// Pattern matching in a let expression
let point4 = (3,5)
let (x,y) = point4
printfn "x=%d, y=%d" x y

// The match expression is the workhorse of F#

let xcoord = 
    match point4 with
    | (x, _) -> x

let swapCoordinates point = 
    match point with 
    | (x, y) -> (y,x)
    
swapCoordinates point4

let swapCoordinates' point = 
    match point with
    | Some (x, y) -> Some (y, x)
    | None -> None

swapCoordinates' (Some point4)

let area shape = 
    match shape with
    | Circle radius -> Math.PI * radius * radius
    | Rectangle (length, width) -> length * width

area circle
area rect

// The same constructor syntax is used to deconstruct
// data in pattern matching
//
// x :: xs     Constructor for a list by adding element to head
// []          Constructor for an empty list
// 
let rec reverse lst =
    match lst with
    | x :: xs -> 
        List.append (reverse xs) [x]
    | [] -> []

// Define reverse without using List module functions
let reverse' xs =
    let rec rev acc xs =    // recursive function must use "let rec"
        match xs with 
        | [] -> acc
        | x :: xs -> rev (x :: acc) xs
    rev [] xs 



// Offside rule
///////////////////////////////////////////////////////////////////////////////

// Must follow indentention rules when using the "lightweight" syntax
let reverse'' xs =
    let rec rev acc xs =
        match xs with 
        | [] -> acc
        | x :: xs -> rev (x :: acc) xs
    rev [] xs 


