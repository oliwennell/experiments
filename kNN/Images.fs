namespace kNN

open System
open System.IO

module Images =

    let classify trainingImages images =  

        let totalSqDistBetween image1 image2 =
            seq { for y in 0..image1.Size-1 do
                    for x in 0..image1.Size-1 do
                        let p1 = (double)image1.GreyscalePixels.[y,x]
                        let p2 = (double)image2.GreyscalePixels.[y,x]
                        yield pown (p2 - p1) 2 }
            |> Seq.sum

        let classifyImage trainingImages image =
            printf "."
            
            let closestNeighbours = trainingImages 
                                    |> Array.map (fun ti -> (ti.Digit, (totalSqDistBetween ti image) ) )   
                                    |> Array.sortBy (fun x -> snd x)
                                    |> Array.take 5
                    
            let mostPopularDigit = closestNeighbours
                                    |> Array.groupBy (fun x -> fst x) 
                                    |> Array.map (fun x -> (fst x, Array.length (snd x)))
                                    |> Array.sortByDescending (fun x -> snd x)
                                    |> Array.item 0
                                    |> fst
            mostPopularDigit

        printfn "Classifying %i images with %i training images" (Array.length images) (Array.length trainingImages)
        { 
            Images = images;
            ClassifiedDigits = images |> Array.map (fun i -> classifyImage trainingImages i) 
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

        seq { for _ in 1 .. numImages -> 
                { 
                    Digit = labelReader.ReadByte(); 
                    GreyscalePixels = readImagePixels(imageSize, imageReader);
                    Size = imageSize
                }
        }
        |> Seq.toArray
