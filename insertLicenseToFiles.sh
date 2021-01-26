# in case of $'r' command not found error run dos2unix on this shell script file to convert line endings to unix
NUMBER_OF_FILES=0

sed '1 i\/**' LICENSE | sed '2,$ s/^/* /' | sed -e '$ a */' > LICENSECOMMENTED
for i in `grep -r -L --exclude-dir="node_modules" --exclude-dir="lib" --exclude-dir="bin" --exclude-dir="obj" --include \*.js --include \*.cs "Finnish Digital Agency (DVV)" .`
do
    sed -i '1s/^\xEF\xBB\xBF//' $i
    echo "$(cat LICENSECOMMENTED)"$'\n'"$(cat $i)" > $i
    ((NUMBER_OF_FILES++))
done
echo $NUMBER_OF_FILES
rm LICENSECOMMENTED

# Mac
# NUMBER_OF_FILES=0

# gsed '1 i\/**' LICENSE | gsed '2,$ s/^/* /' | gsed -e '$ a */' > LICENSECOMMENTED
# for i in `grep -r -L --exclude-dir="*/node_modules/*" --exclude-dir="*/wwwroot/lib/*" --exclude-dir="*/bin/*" --exclude-dir="*/obj/*" --include \*.js --include \*.cs "Finnish Digital Agency (DVV)" .`
# do
#    gsed -i '1s/^\xEF\xBB\xBF//' $i
#    echo "$(cat LICENSECOMMENTED)"$'\n'"$(cat $i)" > $i
#    ((NUMBER_OF_FILES++))
# done
# echo $NUMBER_OF_FILES
# rm LICENSECOMMENTED
