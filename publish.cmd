REM Publish feature files to Augurk

augurk publish --featureFiles "%~dp0\src\Augurk.Specifications\Gherkin" --productName Documentation --groupName Gherkin --url %1 --embed
augurk publish --featureFiles "%~dp0\src\Augurk.Specifications\Configuration" --productName Documentation --groupName Configuration --url %1 --embed

augurk publish --featureFiles "%~dp0\src\Augurk.Specifications\Portal\Versioned Features\V1" --productName Documentation --groupName Portal --version 1.0.0 --url %1 --embed
augurk publish --featureFiles "%~dp0\src\Augurk.Specifications\Portal\Versioned Features\V2" --productName Documentation --groupName Portal --version 2.0.0 --url %1 --embed