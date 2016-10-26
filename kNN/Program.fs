// Learn more about F# at http://fsharp.org

open System
open System.IO

[<AutoOpen>]
module Types =

    type Image = {
        GreyscalePixels: byte[][];
        Digit: byte;
    }

    type ImageSet = {
        Width: int;
        Height: int;
        Images: Image[]
    }

module ImageSet =

    let fromMnistDataset rawImageData rawLabelData =
        
        use imageReader = new BinaryReader(rawImageData)
        use labelReader = new BinaryReader(rawLabelData)

        let readInt32FromMsb (reader : BinaryReader) =  
            let intAsLsbBytes =
                [| reader.ReadByte(); reader.ReadByte(); reader.ReadByte(); reader.ReadByte() |]
                |> Array.rev
            BitConverter.ToInt32(intAsLsbBytes, 0)

        readInt32FromMsb imageReader |> fun x -> if x <> 2051 then raise (System.ArgumentException("Image data not in expected format"))
        readInt32FromMsb labelReader |> fun x -> if x <> 2049 then raise (System.ArgumentException("Label data not in expected format"))

        let numImages = readInt32FromMsb imageReader
        readInt32FromMsb labelReader |> fun x -> if x <> numImages then raise (System.ArgumentException("Mismatch between number of images and number of labels"))

        let imageWidth = readInt32FromMsb imageReader
        let imageHeight = readInt32FromMsb imageReader

        let readImagePixels (imageWidth, imageHeight, reader: BinaryReader) =
            let readPixelRow (imageWidth, reader: BinaryReader) = 
                seq { for x in 1 .. imageWidth -> reader.ReadByte() } |> Seq.toArray 
            
            seq { for y in 1 .. imageHeight -> readPixelRow(imageWidth, reader) } |> Seq.toArray        

        let images = seq { for _ in 1 .. numImages -> 
                            { 
                                Digit = labelReader.ReadByte(); 
                                GreyscalePixels = readImagePixels(imageWidth, imageHeight, imageReader)
                            }
                        }
                        |> Seq.toArray
        
        { 
            Width = imageWidth; 
            Height = imageHeight; 
            Images = images
        }

[<EntryPoint>]
let main argv = 
    printfn "Reading training data..."

    let trainingSet = ImageSet.fromMnistDataset 
                        (File.OpenRead "train-images-idx3-ubyte") 
                        (File.OpenRead "train-labels-idx1-ubyte")

    printfn "There are %i images" trainingSet.Images.Length

    for y in 0 .. trainingSet.Height-1 do
        for x in 0 .. trainingSet.Width-1 do
            printf "%02X" trainingSet.Images.[0].GreyscalePixels.[y].[x]
        printfn ""

    printfn "kthxbye"
    0
