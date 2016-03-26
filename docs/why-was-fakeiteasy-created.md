# Why was FakeItEasy created?

#Introduction

There was a good question on Stack Overflow that asks what
distinguishes FakeItEasy from other frameworks. creator of FakeItEasy,
Patrik H&auml;gne, answered the question there but we reproduce the
answer here. Note that the text has been preserved, and particular
constructs referenced (such as `DummyDefinitions`) have changed or
been renamed in newer versions of FakeItEasy.

The question on Stack Overflow:
"[Are fakes better than Mocks?](http://stackoverflow.com/questions/4001101/are-fakes-better-than-mocks)"

#Patrik H&auml;gne's answer

To be clear, I created FakeItEasy so I'll definitely not say whether
one framework is better than the other, what I can do is point out
some differences and motivate why I created FakeItEasy. Functionally
there are no major differences between Moq and FakeItEasy.

FakeItEasy has no "Verifiable" or "Expectations" it has assertions
however, these are always explicitly stated at the very end of a test,
I believe this makes tests easier to read and understand. It also
helps beginners to avoid multiple asserts (where they would set
expectations on many calls or mock objects).

I used Rhino Mocks before and I quite liked it, especially after the
AAA-syntax was introduced I did like the fluent API of Moq better
though. What I didn't like with Moq was the "mock object" where you
have to use mock.Object everywhere, I like the Rhino-approach with
"natural" mocks better. Every instance looks and feels like a normal
instance of the faked type. I wanted the best of both worlds and also
I wanted to see what I could do with the syntax when I had absolutely
free hands. Personally I (obviously) think I created something that is
a good mix with the best from both world, but that's quite easy when
you're standing on the shoulders of giants.

As has been mentioned here one of the main differences is in the
terminology, FakeItEasy was first created to introduce TDD and mocking
to beginners and having to worry about the differences between mocks
and stubs up front is not very useful.

I've put a lot of focus into the exception messages, it should be very
easy to tell what went wrong in a test just looking at an exception
message.

FakeItEasy has some extensibility features that the other frameworks
don't have but these aren't very well documented yet.

FakeItEasy is (hopefully) a little stronger in mocking classes that
has constructor arguments since it has a mechanism for resolving
dummy-values to use. You can even specify your own dummy value
definitions by implementing a DummyDefinition(Of T) class within your
test project, this will automatically be picked up by FakeItEasy.

The syntax is an obvious difference, which one is better is largely a
matter of taste.

I'm sure there are lots of other differences that I forget about now
(and to be fair I've never used Moq in production myself so my
knowledge of it is limited), I do think these are the most important
differences though.
