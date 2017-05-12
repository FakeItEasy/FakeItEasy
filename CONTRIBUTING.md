# How to Contribute

First of all, thank you for wanting to contribute to FakeItEasy! We really appreciate all the awesome support we get from our community. We want to keep it as easy as possible for you to contribute changes that make FakeItEasy better for you. There are a few guidelines that we need contributors to follow so that we can all work together happily.

## Preparation

Before starting work on a functional change, i.e. a new feature, a change to an existing feature or a bug, please ensure an [issue](https://github.com/FakeItEasy/FakeItEasy/issues) has been raised. Indicate your intention to work on the issue by writing a comment against it. This will prevent duplication of effort. If the change is non-trivial, it's usually best to propose a design in the issue comments.

It is not necessary to raise an issue for non-functional changes, e.g. refactoring, adding tests, reformatting code, documentation, updating packages, etc.

The coordinators will usually assign a priority to each issue from 1 (highest) to 3 (lowest) using the [P1](https://github.com/FakeItEasy/FakeItEasy/labels/P1), [P2](https://github.com/FakeItEasy/FakeItEasy/labels/P2) and [P3](https://github.com/FakeItEasy/FakeItEasy/labels/P3) labels. Feel free to work on any issue you like but bear in mind that the higher the priority of an issue, the earlier the coordinators will direct attention to any comments or pull requests relating to it.

## Tests

Changes in functionality (new features, changed behavior, or bug fixes) should be described by [xBehave.net](http://xbehave.github.io/) acceptance tests in the `FakeItEasy.Specs` project. Doing so ensures that tests are written in language familiar to FakeItEasy's end users and are resilient to refactoring.

There should be a high level of test coverage. When achieving proper coverage is impractical via acceptance tests, then integration tests or unit tests should be added.

When writing integration or unit tests, use the [3A's pattern](http://defragdev.com/blog/?p=783) (Arrange, Act, Assert) with comments indicating each part.
New or changed tests should use [FluentAssertions](https://github.com/dennisdoomen/fluentassertions) for the assertion phase.
E.g.

```c#
// Arrange
var dummy = A.Fake<IFoo>();
A.CallTo(() => this.fakeCreator.CreateDummy<IFoo>()).Returns(dummy);

// Act
var result = A.Dummy<IFoo>();

// Assert
result.Should().BeSameAs(dummy));
```

Whenever FluentAssertions are introduced into a test file, all tests in the file should be converted to use FluentAssertions.

### API Approval

If your contribution changes the public API, you will initially have a test failure from the `FakeItEasy.Tests.Approval` project. In order to fix this, check the difference between `tests\FakeItEasy.Tests.Approval\ApiApproval.ApproveApi.approved.txt` and `tests\FakeItEasy.Tests.Approval\ApiApproval.ApproveApi.received.txt`. If you are satisfied with change, update `...approved.txt` to match `...received.txt` and commit the change.

## Spaces not Tabs

Pull requests containing tabs will not be accepted. Make sure you set your editor to replace tabs with spaces. Indents for all file types should be 4 characters wide with the exception of JSON which should have indents 2 characters wide.

## Line Endings

The repository is configured to preserve line endings both on checkout and commit (the equivalent of `autocrlf` set to `false`). This means *you* are responsible for line endings. We recommend that you configure your diff viewer so that it does not ignore line endings. Any [wall of pink](http://www.hanselman.com/blog/YoureJustAnotherCarriageReturnLineFeedInTheWall.aspx) pull requests will not be accepted.

## Line Width

Try to keep lines of code no longer than 160 characters wide. This isn't a strict rule. Occasionally a line of code can be more readable if allowed to spill over slightly. A good way to remember this rule is to use the 'Column Guides' feature of the [Productivity Power Tools 2012](http://visualstudiogallery.msdn.microsoft.com/3a96a4dc-ba9c-4589-92c5-640e07332afd) extension for Visual Studio.

## Coding Style

Try to keep your coding style in line with the existing code. It might not exactly match your preferred style but it's better to keep things consistent. Coding style compliance is enforced through analysis on each build. Any StyleCop.Analyzers settings changes or suppressions must be clearly justified.

## String Formatting

Unless there is a compelling reason not to, for example when serializing data to be parsed later, use the current culture when formatting strings. When string formatting methods have an overload that implicitly uses the current culture, opt to use it instead of specifying the culture explicitly.

## Code Analysis

Try to avoid introducing new code analysis warnings. Currently the codebase is free of warnings, and we would like to avoid the addition of new warnings. Any code analysis rule changes or suppressions must be clearly justified.

## Resharper Artifacts

Please do not add Resharper suppressions to code using comments. You may tweak your local Resharper settings but do not commit these to the repo.

When configuring e.g. inspection severity, you can choose where to save the settings:

* Solution "FakeItEasy" personal
* Solution "FakeItEasy" team-shared
* This computer

You should pick either the "personal" or "this computer" option.

## Making Changes

FakeItEasy uses the git branching model known as [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/). As such, all development must be performed on a ["feature branch"](https://martinfowler.com/bliki/FeatureBranch.html) created from the main development branch, which is called `develop`. To submit a change:

1. [Fork](http://help.github.com/forking/) the  [FakeItEasy repository](https://github.com/FakeItEasy/FakeItEasy/) on GitHub
1. Clone your fork locally
1. Configure the upstream repo (`git remote add upstream git://github.com/FakeItEasy/FakeItEasy.git`)
1. Create a local branch (`git checkout -b my-branch develop`)
1. Work on your feature
1. Rebase if required (see below)
1. Run code analysis on the solution to ensure you have not introduced any violations
1. Ensure the build succeeds (see ['How to build'](how_to_build.md "How to build"))
1. Push the branch up to GitHub (`git push origin my-branch`)
1. Send a [pull request](https://help.github.com/articles/using-pull-requests) on GitHub

You should **never** work directly on the `develop` branch and you should **never** send a pull request from the `develop` branch - always from a feature branch. The reasons for this are detailed below.

## Handling Updates from upstream/develop

While you're working away in your branch it's quite possible that your upstream/develop (most likely the canonical FakeItEasy version) may be updated. If this happens you should:

1. [Stash](http://progit.org/book/ch6-3.html) any un-committed changes you need to
1. `git checkout develop`
1. `git pull upstream develop`
1. `git checkout my-branch`
1. `git rebase develop my-branch`
1. `git push origin develop` - (optional) this makes sure your remote develop branch is up to date
1. if you previously pushed your branch to your origin, you need to force push the rebased branch - `git push origin my-branch -f`

This ensures that your history is "clean" i.e. you have one branch off from develop followed by your changes in a straight line. Failing to do this ends up with several "messy" merges in your history, which we don't want. This is the reason why you should always work in a branch and you should never be working in, or sending pull requests from, develop.

If you're working on a long running feature then you may want to do this quite often, rather than run the risk of potential merge issues further down the line.

## Sending a Pull Request

While working on your feature you may well create several branches, which is fine, but before you send a pull request you should ensure that you have rebased back to a single feature branch. We care about your commits and we care about your feature branch but we don't care about how many or which branches you created while you were working on it. :smile:

When you're ready to go you should confirm that you are up to date and rebased with upstream/develop (see "Handling Updates from upstream/develop" above) and then:

1. `git push origin my-branch`
1. Send a [pull request](https://help.github.com/articles/using-pull-requests) in GitHub, selecting the following dropdown values:

| Dropdown      | Value                                             |
|---------------|---------------------------------------------------|
| **base fork** | `FakeItEasy/FakeItEasy`                           |
| **base**      | `develop`                                          |
| **head fork** | `{your fork}` (e.g. `{your username}/FakeItEasy`) |
| **compare**   | `my-branch`                                       |

The pull request should include a description starting with "Fixes #123." (using the real issue number, of course) if it fixes an issue. If there's no issue, be sure to clearly explain the intent of the change.
