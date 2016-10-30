namespace kNN

[<AutoOpen>]
module Types =

    type Image = {
        GreyscalePixels: byte[,];
        Size: int;
        Digit: byte;
    }

    type Classification = {
        Images: Image[];
        ClassifiedDigits: byte[]
    }

