# Upgrading from older versions

Here is a regular expression that can help upgrade code written for older versions of FakeItEasy. Specifically, it will replace `Configure.Fake` calls with `A.CallTo` calls:

Find:
`Configure[\.:b\n]*Fake\({[:Al:Nu:Pu]*}\)[\.:b\n]*CallsTo\([:Al] =\> [:Al]\.`

Replace:
`A.CallTo(() => \1.`
