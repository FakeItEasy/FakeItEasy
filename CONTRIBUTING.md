# How to contribute

First of all, thank you for wanting to contribute to FakeItEasy! We really appreciate all the awesome support we get from our community. We want to keep it as easy as possible to contribute changes that get things working in your environment. There are a few guidelines that we need contributors to follow so that we can have a chance of keeping on top of things.

## Preparation

Before starting work on a new bug, feature, etc. make sure that an [issue](https://github.com/FakeItEasy/FakeItEasy/issues) has been raised and that you have written a comment against the issue indicating that you are intending to work on it. This will prevent duplication of effort. If you'd like to implement a new feature, use the issue comments to propose your design before sending a pull request.

## Tests

All features should be described by MSpec feature tests in the FakeItEasy.Specs project. There should also be a high level of unit test coverage. Our target is 95% unit test coverage for the project as a whole (93% at the time of writing). Any new code that is added should also have this level of coverage. For unit tests, use the 3A's pattern (Arrange, Act, Assert) with comments indicating each part. E.g.

    // Arrange
    var dummy = A.Fake<IFoo>();
    A.CallTo(() => this.fakeCreator.CreateDummy<IFoo>()).Returns(dummy);

    // Act
    var result = A.Dummy<IFoo>();

    // Assert
    Assert.That(result, Is.SameAs(dummy));

## Spaces not Tabs

Pull requests containing tabs will not be accepted. Make sure you set your editor to replace tabs with spaces. Indents for all file types should be 4 characters wide with the exception of Ruby (rakefile.rb) which should have indents 2 characters wide.

## Line Width

Try to keep lines of code no longer than 160 characters wide. This isn't a strict rule. Occasionally a line of code can be more readable if allowed to spill over slightly. A good way to remember this rule is to use the 'Column Guides' feature of the [Productivity Power Tools 2012](http://visualstudiogallery.msdn.microsoft.com/3a96a4dc-ba9c-4589-92c5-640e07332afd) extension for Visual Studio.

## Coding style

Try to keep your coding style in line with the rest of the codebase. It might not exactly match your preferred style but it's better to keep the codebase consistent. The code is not currently StyleCop compliant but this in the pipeline.

## Code Analysis

Try and avoid introducing new code analysis warnings. Currently the codebase has quite a few warnings, which we would like to address, and we would like to avoid the addition of new warnings.

## Resharper Suppressions

Please do not add Resharper suppressions using comments. If you are using Resharper, you may tweak your local settings to reduce false positives but do not commit these to the repo. A definitive Resharper settings file commited to the repo is in the pipeline.

## Making Changes

1. [Fork](http://help.github.com/forking/) the [FakeItEasy repository](https://github.com/FakeItEasy/FakeItEasy/) on GitHub
1. Make sure your you set autoclrf to true. `git config --local core.autocrlf true`
1. Clone your fork locally
1. Configure the upstream repo (`git remote add upstream git://github.com/FakeItEasy/FakeItEasy.git`)
1. Create a local branch (`git checkout -b myBranch`)
1. Work on your feature
1. Rebase if required (see below)
1. Push the branch up to GitHub (`git push origin myBranch`)
1. Send a Pull Request on GitHub

You should **never** work on a clone of master, and you should **never** send a pull request from master - always from a branch. The reasons for this are detailed below.

## Handling Updates from Upstream/Master

While you're working away in your branch it's quite possible that your upstream master (most likely the canonical FakeItEasy version) may be updated. If this happens you should:

1. [Stash](http://progit.org/book/ch6-3.html) any un-committed changes you need to
1. `git checkout master`
1. `git pull upstream master`
1. `git checkout myBranch`
1. `git rebase master myBranch`
1. `git push origin master` - (optional) this this makes sure your remote master is up to date

This ensures that your history is "clean" i.e. you have one branch off from master followed by your changes in a straight line. Failing to do this ends up with several "messy" merges in your history, which we don't want. This is the reason why you should always work in a branch and you should never be working in, or sending pull requests from, master.

If you're working on a long running feature then you may want to do this quite often, rather than run the risk of potential merge issues further down the line.

## Sending a Pull Request

While working on your feature you may well create several branches, which is fine, but before you send a pull request you should ensure that you have rebased back to a single "Feature branch" - we care about your commits, and we care about your feature branch; but we don't care about how many or which branches you created while you were working on it :-)

When you're ready to go you should confirm that you are up to date and rebased with upstream/master (see "Handling Updates from Upstream/Master" above), and then:

1. `git push origin myBranch`
1. Send a descriptive [Pull Request](http://help.github.com/pull-requests/) on GitHub. Make sure you select `FakeItEasy/FakeItEasy` as the *base repo* and `master` as the *base branch*. Select your fork as the *head repo* and your branch as the *head branch*.
1. If GitHub indicates that the pull request can be merged automatically, check that the pull request has built successfully on our [CI server](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt929) (you can either create an account or login as guest). E.g. if your pull request number is 123, you should see a build labelled '123/merge'. If the build has failed (red) then there is a problem with your changes and you'll have to address it before the pull request is accepted. You can inspect the build logs to see what caused the failure.