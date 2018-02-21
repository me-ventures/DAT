git checkout release
git merge --no-ff master
echo "export VERSION=$1" > VERSION
git commit .
git commit -am "Version bump to $VERSION"
git tag -a $VERSION -m "$VERSION Release"
