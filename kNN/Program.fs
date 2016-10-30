
open kNN
open System
open System.IO

let printClassification classification =
    
    let printSingleImage imageIndex = 
        let image = classification.Images.[imageIndex]
        let actual = image.Digit
        printfn "Image is %i" actual
        
        for row in 0 .. image.Size-1 do
            for column in 0 .. image.Size-1 do
                printf "%02X" image.GreyscalePixels.[row,column]
            printfn ""
        
        let classifiedAs = classification.ClassifiedDigits.[imageIndex]
        let wasCorrect = actual = classifiedAs

        printfn "... and has been classified as %i => %s" classifiedAs (if wasCorrect then "true" else "FALSE")
        printfn ""
        
        wasCorrect

    seq { for i in 0 .. classification.Images.Length-1 do yield printSingleImage i }

let calculateAccuracy results =
    let numSuccess = Seq.where (fun r -> r) results |> Seq.length
    let total = Array.length results
    (float)(numSuccess / total) * 100.0

[<EntryPoint>]
let main argv = 
    printfn "Reading training data..."

    use trainingImageData = File.OpenRead "train-images-idx3-ubyte" 
    use trainingLabelData = File.OpenRead "train-labels-idx1-ubyte"
    let trainingImages = Images.fromMnistDataset trainingImageData trainingLabelData
    printfn "There are %i training images" trainingImages.Length

    use testImageData = File.OpenRead "t10k-images-idx3-ubyte" 
    use testLabelData = File.OpenRead "t10k-labels-idx1-ubyte"
    let testImages = Images.fromMnistDataset testImageData testLabelData
    printfn "There are %i test images" testImages.Length

    testImages
    |> Images.classify
    |> printClassification
    |> Seq.toArray
    |> calculateAccuracy
    |> printfn "Accuracy: %1.2f%%"

    printfn "kthxbye"
    0
