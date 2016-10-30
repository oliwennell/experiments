namespace kNN

open System
open System.IO

module Images =

    let classify images =  

        let totalDistanceBetween image1 image2 =
            seq { for y in 0..image1.Size-1 do
                    for x in 0..image1.Size-1 do
                        yield pown ((double)image2.GreyscalePixels.[y,x] - (double)image1.GreyscalePixels.[y,x]) 2 }
            |> Seq.sum

        let classifyImage trainingImages image =
            trainingImages 
            |> Array.map (fun ti -> (ti.Digit, (totalDistanceBetween ti image) ) )   
            |> Array.sortBy (fun x -> snd x)
            |> Array.groupBy (fun x -> fst x)
            |> Array.map (fun x -> (fst x, Array.length (snd x)))
            |> Array.sortByDescending (fun x -> snd x)
            |> Array.item 0
            |> fst
            
        { 
            Images = images;
            ClassifiedDigits = images |> Array.map (fun i -> classifyImage images i) 
        }
        
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

        let imageSize = readInt32FromMsb imageReader
        readInt32FromMsb imageReader |> ignore

        let readImagePixels (imageSize, reader: BinaryReader) =
            let allPixels = seq { for y in 1 .. imageSize do
                                    for x in 1 .. imageSize do
                                        yield reader.ReadByte() }
                            |> Seq.toArray
                            
            Array2D.init imageSize imageSize (fun y x -> allPixels.[(y*imageSize)+x])  

        //let images = seq { for _ in 1 .. numImages -> 
        seq { for _ in 1 .. 3 -> 
                { 
                    Digit = labelReader.ReadByte(); 
                    GreyscalePixels = readImagePixels(imageSize, imageReader);
                    Size = imageSize
                }
        }
        |> Seq.toArray
