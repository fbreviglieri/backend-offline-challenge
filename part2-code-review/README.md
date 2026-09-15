# Part 2 — Code Review

## The code in question

```csharp
if (message is MessageA)
{
    var messageA = message as MessageA;
    messageA?.MyCustomMethodOnA();
}
else if (message is MessageB)
{
    var messageB = message as MessageB;
    messageB?.MyCustomMethodOnB();
    messageB?.SomeAdditionalMethodOnB();
}
else if (message is MessageC)
{
    var messageC = message as MessageC;
    messageC?.MyCustomMethodOnC();
}
```

## Rating: poor / not production quality

It "works", but it has several concrete problems:

1. **Violates the Open/Closed Principle.** Every time a new message type (`MessageD`, …) is
   introduced, this method must be edited. In a real codebase this kind of dispatch block tends
   to be duplicated in several places, so one new message type means hunting down and editing
   all of them.
2. **No compiler-enforced exhaustiveness.** If a `MessageD` is added and this block is
   forgotten, the code silently does nothing for it — falls through with no warning, no error,
   just a quietly-dropped message. That's a production incident waiting to happen.
3. **Redundant type test + cast.** `is` followed by `as` re-does the type check that pattern
   matching does in one step, and the resulting `?.` silently swallows the (impossible) null
   case instead of it being statically impossible.
4. **Double dispatch by hand.** The method is really asking "what is the runtime type of
   `message`, and what should happen for that type" — that's exactly the problem polymorphism
   solves. Doing it with a type-check chain instead of a virtual call pushes behavior for `A`,
   `B`, `C` into a place that knows about all three, instead of letting each message type own
   its own behavior.
5. **Not unit-testable in isolation.** Testing "what happens for a MessageB" means invoking this
   whole block; there's no seam to test each branch independently.

## Recommended alternative: polymorphism

If `MessageA/B/C` share a common base (or can be given one), push the behavior onto the types
themselves and let the runtime do the dispatch:

```csharp
public interface IMessage
{
    void Handle();
}

public sealed class MessageA : IMessage
{
    public void Handle() => MyCustomMethodOnA();
    private void MyCustomMethodOnA() { /* ... */ }
}

public sealed class MessageB : IMessage
{
    public void Handle()
    {
        MyCustomMethodOnB();
        SomeAdditionalMethodOnB();
    }
    private void MyCustomMethodOnB() { /* ... */ }
    private void SomeAdditionalMethodOnB() { /* ... */ }
}

public sealed class MessageC : IMessage
{
    public void Handle() => MyCustomMethodOnC();
    private void MyCustomMethodOnC() { /* ... */ }
}
```

Call site collapses to:

```csharp
message.Handle();
```

Adding `MessageD` means adding a new class that implements `IMessage` — nothing else in the
codebase changes, and there is no chain of type checks to fall out of sync. Each message's
behavior is independently unit-testable.

## Alternative when you don't own the message types

If `MessageA/B/C` are external/generated types you can't add an interface to (or the "handling"
genuinely belongs outside the message, e.g. in a dispatcher/visitor), C# pattern matching on a
sealed hierarchy is a reasonable middle ground — it still removes the manual `is`/`as`
boilerplate. Written as a switch **expression** (rather than a switch statement) over a
`sealed`/closed base type, the compiler emits warning `CS8509` if a case is missing, which a
plain switch *statement* will not do — so prefer the expression form here specifically to get
that exhaustiveness check:

```csharp
Action handle = message switch
{
    MessageA a => a.MyCustomMethodOnA,
    MessageB b => () => { b.MyCustomMethodOnB(); b.SomeAdditionalMethodOnB(); },
    MessageC c => c.MyCustomMethodOnC,
    _ => throw new NotSupportedException($"Unhandled message type: {message.GetType()}"),
};
handle();
```

The `_ => throw` arm is the important part regardless of form — it converts "silently do nothing
for an unknown type" into a loud, immediate failure, which is far safer in production than the
original code's silent fall-through. The compiler warning is a bonus that only the expression
form gives you; don't rely on it in place of the explicit throw, since a non-sealed hierarchy or
a switch statement won't get it.

**Preference**: polymorphism (`IMessage.Handle()`) is the stronger fix because it removes the
dispatch problem entirely rather than just making its failure mode louder. Use the switch-based
approach only when you don't control the message types.
