# Bank OCR code challenge

## Introduction

Details of requirements can be found [here](https://code.joejag.com/coding-dojo/bank-ocr/).

## Running the project

1. Open BankOcr.sln in Visual Studio
2. Ensure BankOcr.ConsoleApp is the start up project
3. Build and run

BankOcr.ConsoleApp is a console app that has a minimal GUI that allows you to select one of the sample files found in BankOcr\BankOcr.ConsoleApp\ocr-files.

If you wish to add to the list of files follow these steps;
1. Create a new text file, add whatever contents you want
2. Add the file to BankOcr\BankOcr.ConsoleApp\ocr-files
3. In Visual Studio, right click the file and select "Properties"
4. For "Copy to Output Directory" choose "Copy always"
5. Re-run the solution
6. Your new file should be in the list to select from

If you want to run the unit tests they are in the BankOcr.Business.Tests project, they are;
* AccountNumber
	* AccountNumberIsValid
	* GetValidAccountNumbers
* Ocr
	* ConvertOcrDigitToNumber
	* ConvertOcrNumberToAccountNumber
	* GetAccountNumbersFromOcrFileContents
	* GuessOcrDigit
	* ValidateOcrFile