{-# LANGUAGE DataKinds #-}
{-# LANGUAGE GeneralizedNewtypeDeriving #-}
module Main where

import Crypto.Classes
import Data.Digest.Pure.MD5
import Data.ByteString.Lazy (toStrict)
import Data.ByteString.Builder (toLazyByteString, stringUtf8)
import Data.Proxy
import Data.Maybe
import Data.Foldable
import Data.Map.Strict (Map)
import qualified Data.Map.Strict as Map
import Graphics.Identicon
import Graphics.Identicon.Styles.Squares
import Codec.Picture
import Codec.Picture.Types (Image, PixelRGB8)
import Control.Monad
import Control.Applicative (Alternative, (<|>), empty)
import Control.Monad.Trans.Maybe (MaybeT(..))
import System.Environment
import Text.Read

data User = User {
    userID :: Int
  , userName :: String
  , userEmail :: String
  , useGravatar :: Bool
  , useIdenticon :: Bool }
  deriving (Eq, Show)

-- An icon is here just a wrapped URL that can be used in an img tag.
newtype Icon = Icon String deriving (Show, Eq)

hashString :: Hash ctx d => String -> d
hashString = hash . toLazyByteString . stringUtf8

--------------
-- Gravatar --
--------------

gravatarUrl :: String -> String
gravatarUrl email =
  "https://www.gravatar.com/avatar/" ++ show (hashString email :: MD5Digest)

getGravatar :: User -> Maybe Icon
getGravatar u =
  if useGravatar u
    then Just $ Icon $ gravatarUrl $ userEmail u
    else Nothing

---------------
-- Identicon --
---------------

identiconUrl :: User -> String
identiconUrl (User i n e _ _) =
  "https://example.com/identicon/" ++ show (hashString (show i ++ n ++ e) :: MD5Digest)

getIdenticon :: User -> Maybe Icon
getIdenticon u =
  if useIdenticon u
    then Just $ Icon $ identiconUrl u
    else Nothing

-- Here's what a server-side handler of the above identicon URL could do to
-- generate an identicon:
generateIdenticon :: String -> Maybe (Image PixelRGB8)
generateIdenticon bytesAsString =
  genIcon 80 80 $ toStrict $ toLazyByteString $ stringUtf8 bytesAsString
  where
    genIcon =
      renderIdenticon (Proxy :: Proxy (Squares 2)) $ squares (Proxy :: Proxy 2)

-- Only for ad-hoc testing purposes, here's a helper function to save an image
-- to a file:
saveIdenticon :: FilePath -> Image PixelRGB8 -> IO ()
saveIdenticon fileName img = savePngImage fileName $ ImageRGB8 img

-------------------
-- User database --
-------------------

getDBIcon :: Map Int a -> User -> Maybe a
getDBIcon m u = Map.lookup (userID u) m

db :: Map Int Icon
db =
  Map.fromList [
    (42, Icon "https://example.com/users/42/icon"),
    (90125, Icon "https://example.com/users/90125/icon"),
    (666, Icon "https://example.com/users/666/icon")
  ]

------------------
-- Default icon --
------------------

defaultIcon :: Icon
defaultIcon = Icon "https://example.com/default-icon"

--------------------
-- Lazy IO monoid --
--------------------

newtype FirstIO a =
  FirstIO (MaybeT IO a) deriving (Functor, Applicative, Monad, Alternative)

firstIO :: IO (Maybe a) -> FirstIO a
firstIO = FirstIO . MaybeT

getFirstIO :: FirstIO a -> IO (Maybe a)
getFirstIO (FirstIO (MaybeT x)) = x

instance Semigroup (FirstIO a) where
  (<>) = (<|>)

instance Monoid (FirstIO a) where
  mempty = empty

---------------
-- Alignment --
---------------

-- This section of the code aligns the above Lego pieces to a common abstraction

trace :: Show a => FirstIO a -> FirstIO a
trace fio = firstIO $ do
  x <- getFirstIO fio
  case x of
    Just x' -> putStrLn $ "Returning " ++ show x'
    Nothing -> putStrLn "Found nothing"
  return x

getGravatarIO :: User -> FirstIO Icon
getGravatarIO = trace . firstIO . return . getGravatar

getIdenticonIO :: User -> FirstIO Icon
getIdenticonIO = trace . firstIO . return . getIdenticon

getDBIconIO :: User -> FirstIO Icon
getDBIconIO = trace . firstIO . return . getDBIcon db

-----------------
-- Composition --
-----------------

getIcon :: User -> IO Icon
getIcon u = do
  let lazyIcons = fmap (\f -> f u) [getGravatarIO, getIdenticonIO, getDBIconIO]
  -- Actually, mconcat is enough, but fold may better illustrate that this step
  -- is also a catamorphism.
  m <- getFirstIO $ fold lazyIcons
  return $ fromMaybe defaultIcon m
-- Alternative, point-free implementation, if that's more to your liking:
-- getIcon = fmap (fromMaybe defaultIcon) . getFirstIO . fold [getGravatarIO, getIdenticonIO, getDBIconIO]

-----------------
-- Entry point --
-----------------

main :: IO ()
main = do
  args <- getArgs
  case args of
    [i, n, e, g, ii] -> do
      let mu = User <$> readMaybe i <*> pure n <*> pure e <*> readMaybe g <*> readMaybe ii
      icon <- traverse getIcon mu
      forM_ icon print
    _ -> putStrLn "Required arguments: User ID, name, email, use Gravatar, use Identicon"
