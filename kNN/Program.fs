// Learn more about F# at http://fsharp.org

open System
open System.IO

type TrainingImage = {
    GrayscalePixels: byte[];
    DigitRepresented: byte;
}

type TrainingSet = {
    Width: int;
    Height: int;
    Images: TrainingImage[]
}

let trainingSetFromMnistDataset (rawImageData:Stream, rawLabelData:Stream) =
    
    use imageReader = new BinaryReader(rawImageData)
    use labelReader = new BinaryReader(rawLabelData)

    let readInt32FromMsb (reader:BinaryReader) =  
        let intAsLsbBytes =
            [| reader.ReadByte(); reader.ReadByte(); reader.ReadByte(); reader.ReadByte() |]
            |> Array.rev
        BitConverter.ToInt32(intAsLsbBytes, 0)

    readInt32FromMsb imageReader |> fun x -> if x <> 2051 then raise (System.ArgumentException("Image data not in expected format"))
    readInt32FromMsb labelReader |> fun x -> if x <> 2049 then raise (System.ArgumentException("Label data not in expected format"))

    let numImages = readInt32FromMsb imageReader
    readInt32FromMsb labelReader |> fun x -> if x <> numImages then raise (System.ArgumentException("Mismatch between number of images and number of labels"))

    printfn "Number of images: %i" numImages

    { Width=1; Height=1; Images=Array.empty }

[<EntryPoint>]
let main argv = 
    printfn "Hello..."

    use rawImageData = File.OpenRead "train-images-idx3-ubyte"
    use rawLabelData = File.OpenRead "train-labels-idx1-ubyte"
    let trainingSet = trainingSetFromMnistDataset (rawImageData, rawLabelData)

    printfn "...world"
    0
