This experiment takes in images of hand-drawn digits and determines what digit is represented, using the k Nearest Neighbor algorithm.

Both the input images and the images used as training data are from [the MNIST database](http://yann.lecun.com/exdb/mnist/).

# Prerequisites

* dotnet core 1.0 or higher

# Running

`$ dotnet run`

You can optionally specify which images to classify, of the 10,000 available:

`$ dotnet run <start> <number>`

E.g. to classify the last 100:

`$ dotnet run 9900 100`
