if [ -z "$1" ]
then
    echo "Version not specified."
    exit 1
fi

git checkout release
git merge --no-ff master
echo "export VERSION=$1" > VERSION
git add .
git commit -am "Version bump to $1"
git tag -a $1 -m "$1 Release"
git push origin release
git checkout master
