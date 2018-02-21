git checkout release
git merge --no-ff master
echo "export VERSION=$1" > VERSION
git commit .
git commit -am "Version bump to $1"
git tag -a $1 -m "$1 Release"
