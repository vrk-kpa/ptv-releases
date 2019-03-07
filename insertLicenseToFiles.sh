NUMBER_OF_FILES=0

gsed '1 i\/**' LICENSE | gsed '2,$ s/^/* /' | gsed -e '$ a */' > LICENSECOMMENTED
for i in `grep -r -L --exclude-dir="*/node_modules/*" --exclude-dir="*/wwwroot/lib/*" --exclude-dir="*/bin/*" --exclude-dir="*/obj/*" --include \*.js --include \*.cs "Population Register Centre (VRK)" .`
do
    gsed -i '1s/^\xEF\xBB\xBF//' $i
    echo "$(cat LICENSECOMMENTED)"$'\n'"$(cat $i)" > $i
    ((NUMBER_OF_FILES++))
done
echo $NUMBER_OF_FILES
rm LICENSECOMMENTED