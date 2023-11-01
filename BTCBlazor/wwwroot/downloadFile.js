function downloadFile(filePath)
{
    var anchor = document.createElement('a');
    anchor.href = filePath;
    anchor.download = filePath.split('/').pop();
    anchor.click();
}
